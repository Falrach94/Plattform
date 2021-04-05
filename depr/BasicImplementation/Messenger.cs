using GameServer.Data_Objects;
using LogUtils;
using MessageUtilities;
using NetworkUtils.Socket;
using ServerImplementation;
using ServerImplementation.Exceptions;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class Messenger : IMessageHandler, IMessenger
    {
        class Response
        {
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

            private Message _msg = null;

            public Message Message
            {
                get => _msg;
                set
                {
                    _msg = value;
                    _semaphore.Release();
                }
            }

            public async Task<Message> WaitForMessage(int timeout)
            {
                if(!await _semaphore.WaitAsync(timeout))
                {
                    throw new TimeoutException();
                }
                return _msg;
            }

        }

        #region fields
        private int _nextId = 0;

        private readonly SemaphoreSlim _responseMutex = new(1,1);
        private readonly IDictionary<Tuple<IEndpoint, int>, Response> _responseDic = new Dictionary<Tuple<IEndpoint, int>, Response>();

        #endregion

        #region logger
        private static readonly Logger systemLogger = Logging.LogManager.GetLogger("System");
        private static readonly Logger messageLogger = Logging.LogManager.GetLogger("Messages");
        #endregion

        #region properties

        public IConnectionManager ConnectionHandler { get; set; }
        public IConnectionStorage Connections { get; set; }
        public IModuleControl Modules { get; set; }

        #endregion

        #region messenger functions


        public async Task BroadcastMessage(Message msg)
        {
            await BroadcastMessage(Connections.Connections, msg);
        }
        public async Task BroadcastMessage(IEnumerable<IConnection> receiver, Message msg)
        {
            receiver = new List<IConnection>(receiver);

            List<Task> tasks = new List<Task>();

            foreach (var c in receiver)
            {
                if (c.State == ConnectionState.Connecting
                || c.State == ConnectionState.Disconnected)
                {
                    continue;
                }
                tasks.Add(SendMessage(c, msg));
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (NotConnectedException)
            {
            }
        }

        public async Task<bool> BroadcastMessageAndWait(IEnumerable<IConnection> receiver, Message msg, int timeoutMs)
        {
            receiver = new List<IConnection>(receiver);

            List<Task<Message>> tasks = new List<Task<Message>>();

            foreach (var c in receiver)
            {
                if (c.State == ConnectionState.Connecting
                || c.State == ConnectionState.Disconnected)
                {
                    continue;
                }
                tasks.Add(SendAndWaitForResponse(c, msg, timeoutMs, null));
            }
            try
            {
                await Task.WhenAll(tasks);

                return !tasks.Where(m => m.Result.Type != "Success").Any();
            }
            catch (NotConnectedException)
            {
                return false;
            }
        }
        public async Task<bool> SendMessage(IConnection receiver, Message msg)
        {
            return await SendMessage(receiver, msg, _nextId++);
        }

        private async Task<bool> SendMessage(IConnection receiver, Message msg, int id)
        {
            msg.Id = id;

            if (await receiver.Endpoint.Send(await msg.Serialize()))
            {
                messageLogger.Debug("message sent " + receiver + " " + msg);
                return true;
            }
            return false;
        }

        /** 
         * @brief Sends message to specified receiver and sets thread to sleep till response message arrives.
         * @param receiver: com connection 
         * @param msg: message to be sent
         * @param timeoutMs: timeout in ms after wich an ResponseTimeoutException will be thrown
         * @param responseType: type of the excpexted response; if response deviates from this value a WrongResponseTypeException will be thrown; (-1 if all message types should be accepted)
         * 
         * @retval
         */
        public async Task<Message> SendAndWaitForResponse(IConnection receiver, Message msg, int timeoutMs, string responseType)
        {
            int id = _nextId++;

            var response = new Response();

            var key = Tuple.Create(receiver.Endpoint, id);

            await _responseMutex.WaitAsync();
            {
                _responseDic.Add(key, response);
            }
            _responseMutex.Release(); 

            await SendMessage(receiver, msg, id);

            messageLogger.Debug(receiver + " waiting for response ("+responseType +")");

            Message responseMsg;
            try
            {
                responseMsg = await response.WaitForMessage(timeoutMs);
            }
            finally
            {
                await _responseMutex.WaitAsync();
                {
                    _responseDic.Remove(key);
                }
                _responseMutex.Release();
            }
            if (responseType != null
            && responseType != responseMsg.Type)
            {
                throw new WrongResponseTypeException(responseType, responseMsg.Type);
            }

            return responseMsg;
        }

        public async Task<bool> Respond(IConnection receiver, Message original, Message response)
        {
            response.ResponseId = original.Id;
            return await SendMessage(receiver, response);
        }

        #endregion

        #region parser functions

        private async Task HandleResponse(IConnection connection, Message msg)
        {
            var key = Tuple.Create(connection.Endpoint, msg.ResponseId);
            await _responseMutex.WaitAsync();
            {
                if (_responseDic.ContainsKey(key))
                {
                    _responseDic[key].Message = msg;
                    _responseDic.Remove(key);
                }
                else
                {
                    messageLogger.Warn("Received response message without corresponding consumer!");
                }
            }
            _responseMutex.Release();
        }

        public async Task<Message> ParseMessage(byte[] msg)
        {
            using var stream = new MemoryStream(msg);
            return await JsonSerializer.DeserializeAsync<Message>(stream);
        }

        public async Task HandleMessage(IConnection connection, Message msg)
        {
            try
            {
                messageLogger.Debug("received " + connection + ": " + msg);

                using (var token = await connection.StateMutex.LockAsync(Const.DEADLOCK_TIMEOUT))
                {
                    if (connection.State == ConnectionState.Connecting
                    || connection.State == ConnectionState.Disconnected)
                    {
                        throw new InvalidConnectionStateException(connection.State);
                    }
                }

                if (msg.IsResponse)
                {
                    await HandleResponse(connection, msg);
                }
                else
                {
                    if (connection.State == ConnectionState.Synchronizing)
                    {
                        throw new InvalidConnectionStateException(connection.State);
                    }
                    if (msg.Module == null)
                    {
                        throw new ProtocolViolationException("Module name must not be null!");
                    }
                    await Modules.DistributeMessage(connection, msg);
                }
            }
            catch (InvalidConnectionStateException ex)
            {
                messageLogger.Info("protocol violation: client sent message on endpoint in state " + ex.State);
                await ConnectionHandler.CloseConnection(connection, Network.DisconnectReason.ProtocolViolation, "Clients may not send messages during " + ex.State + " phase.");
            }
            catch (Exception ex)
            {
                systemLogger.Error("Message handling caused unexpected exception!\n" + ex.ToString());
                await ConnectionHandler.CloseConnection(connection, Network.DisconnectReason.InternalError, "Unexpected behaviour!");
            }
            
        }

        public Task RespondWithError(IConnection receiver, Message original, int error, string desc)
        {
            var data = new SerializableStorage();
            data.Add("Data", Tuple.Create(error, desc));
            return Respond(receiver, original, new Message(null, "Error", data));
        }
        public Task RespondWithError(IConnection receiver, Message original, Enum error, string desc)
        {
            return RespondWithError(receiver, original, (int)(object)error, desc);
        }

        public Task RespondWithSuccess(IConnection receiver, Message original)
        {
            return RespondWithSuccess(receiver, original, null);
        }
        public Task RespondWithSuccess(IConnection receiver, Message original, object result)
        {
            var data = new SerializableStorage();
            data.Add("Data", result);
            return Respond(receiver, original, new Message(null, "Success", data));
        }



        #endregion


    }
}