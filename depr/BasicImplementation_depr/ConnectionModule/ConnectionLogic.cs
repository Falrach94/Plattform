using GameServer;
using GameServer.Data_Objects;
using GameServer.Network;
using GameServerData;
using NetworkUtils.Socket;
using ServerImplementation.Client;
using ServerImplementation.ConnectionHandler;
using ServerImplementation.Exceptions;
using ServerImplementation.Modules;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerImplementation.ConnectionModule
{
    public class ConnectionLogic : ModuleLogic<ConnectionData>, IConnectionHandler
    {

        private static readonly GeneralMessageFactory Messages = GeneralMessageFactory.GetInstance();

        #region properties
        IConnectionStorage IConnectionHandler.Data => Data;
        public IModuleControl Modules { get; set; }
        #endregion

        public override async Task Reset()
        {
            var connections = Data.Connections.ToArray();
            foreach (var c in connections)
            {
                await CloseConnection(c, DisconnectReason.ServerClosed, "Server closed!");
            }
        }
        public async Task CloseConnection(IConnection client, DisconnectReason reason, string message)
        {
            
            await Messenger.SendMessage(client, Messages.ServerClosedConnection(message, reason));

            await client.Endpoint.Disconnect();

            log.Info("closed endpoint (ep: " + client.Id + ", " + reason + ", " + (message??"") + ")");
        }

        #region endpoint proccessing

        public async Task ProcessEndpoint(IConnection connection, EndpointChangedEventArgs e)
        {
            log.Trace("process connection " + connection + "  event (" + e.Type + ")");
            try
            {
                switch (e.Type)
                {
                    case EndpointEventType.Connect:
                        try
                        {
                            await EndpointConnected(e.Endpoint);
                        }
                        catch
                        {
                            if (connection.Endpoint.Connected)
                            {
                                await CloseConnection(connection, DisconnectReason.InternalError, "");
                            }
                            throw;
                        }
                        break;
                    case EndpointEventType.Disconnect:
                        await EndpointDisconnected(e.Endpoint, e.DisconnectArgs);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (EndpointDisconnectedException)
            {
                log.Debug(e.Type + " aborted");
            }
            catch (Exception ex)
            {
                log.Error( ex.ToString(), ex);
            }
        }

        private async Task EndpointConnected(IEndpoint ep)
        {
            var connection = ep.Connection as Connection;

            log.Debug("new connection " + connection);

            await Messenger.SendMessage(connection, Messages.Hello(connection));

            await Broadcast(Messages.NewConnection(connection));

            //synchonize with exisiting and expose to new connection broadcasts
            using (var aLock = await Mutex.Lock(Const.DEADLOCK_TIMEOUT))
            {
                _ = Messenger.SendMessage(connection, Messages.AllConnections(Data.Connections));
                Data.AddConnection(connection);
                _ = Data.SetConnectionState(connection, ConnectionState.Synchronizing);
            }


            try
            {              
                //match loaded modules of client with server
                var moduleMsg = await Messenger.SendAndWaitForResponse(connection, Messages.Modules(Modules.LoadedModules), Const.RESPONSE_TIMEOUT, GeneralClientMessageType.Modules.ToString());
                var modules = moduleMsg.Data.Get<List<string>>("Modules");
                foreach (var m in Modules.LoadedModules)
                {
                    connection.ModuleDic.Add(m.Name, modules.Contains(m.Name));
                }

                //synchronize all matching modules with client
                await Modules.SynchronizeClient(connection);

              
                //finalize connection 
                await Messenger.SendAndWaitForResponse(connection, Messages.ConnectFinished(connection.Id), Const.RESPONSE_TIMEOUT, GeneralClientMessageType.Ready.ToString());

                await Data.SetConnectionState(connection, ConnectionState.Established);
                log.Info(connection + " finished connecting");
            }
            catch (TimeoutException)
            {
                await CloseConnection(connection, DisconnectReason.Timeout, "Timeout while waiting for message!");
                throw new EndpointDisconnectedException();
            }
            catch (WrongResponseTypeException ex)
            {
                await CloseConnection(connection, DisconnectReason.ProtocolViolation, "Received '" + ex.ReceivedType + "' instead of '" + ex.ExpectedType + "'!");
                throw new EndpointDisconnectedException();
            }

        }

        private async Task EndpointDisconnected(IEndpoint endpoint, DisconnectedArgs args)
        {
            log.Info("endpoint disconnected (id: " + endpoint.Connection.Id + ", by " + (args.RemoteDisconnect ? "client" : "server") + ")");

            Connection connection = endpoint.Connection as Connection;

            //wait for connection to pass 'connecting' state
            using (var aLock = await connection.StateMutex.Lock(Const.DEADLOCK_TIMEOUT))
            {
                while (connection.State == ConnectionState.Connecting)
                {
                    await connection.StateMutex.Wait(aLock, Const.DEADLOCK_TIMEOUT);
                }
            }

            Data.RemoveConnection(connection);

            using (var aLock = await Mutex.Lock(Const.DEADLOCK_TIMEOUT))
            {
                _ = Messenger.BroadcastMessage(Messages.ClientDisconnected(connection));
                _ = Data.SetConnectionState(connection, ConnectionState.Disconnected);
            }

            await Modules.HandleDisconnect(connection);

        }

        #endregion

    }
}