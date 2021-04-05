
using GameServer.Data_Objects;
using NetworkUtils.Socket;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GameServer
{
    public class Endpoint : IEndpoint
    {
        #region fields
        #endregion

        #region properties

        public IConnection Connection { get; }
        public ISocket Socket { get; }
        public bool Connected => (Socket != null) && Socket.IsConnected;

        public TransformBlock<byte[], Tuple<IEndpoint, byte[]>> MessageBlock { get; }
        #endregion

        public event EventHandler<DisconnectedArgs> Disconnected;

        public async Task<bool> Send(byte[] data)
        {
            return await Socket.SendAsync(data);
        }

        public Endpoint(ISocket socket)
        {
            Connection = new Connection(this);

            Socket = socket;
            Socket.Disconnected += DisconnectedHandler;

            MessageBlock = new(TransformMessage);
            Socket.MessageBlock.LinkTo(MessageBlock);
        }

        private Tuple<IEndpoint, byte[]> TransformMessage(byte[] buffer)
        {
            return Tuple.Create((IEndpoint)this, buffer);
        }

        private void DisconnectedHandler(object socket, DisconnectedArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        public async Task DisconnectAsync()
        {
            await Socket.Disconnect();
        }
    }
}
