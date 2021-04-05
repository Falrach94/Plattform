
using GameServer.Data_Objects;
using GameServerData;
using NetworkUtils.Socket;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class Endpoint : IEndpoint
    {
        #region fields
        #endregion

        #region properties
        public Thread ReceiveThread { get; set; }
        public CancellationTokenSource CancelSource { get; } = new CancellationTokenSource();
        public IConnection Connection { get; }
        public ISocket Socket { get; }
        public bool Connected => (Socket != null) && Socket.IsConnected;
        #endregion

        public event EventHandler<DisconnectedArgs> Disconnected;

        public async Task<bool> Send(byte[] data)
        {
            return await Socket.Send(data);
        }

        public Endpoint(Socket socket)
        {
            Connection = new Connection(this);

            Socket = socket;
            Socket.Disconnected += DisconnectedHandler;
        }

        private void DisconnectedHandler(object socket, DisconnectedArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        public async Task Disconnect()
        {
            await Socket.Disconnect();
        }
    }
}
