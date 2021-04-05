using GameServer.Data_Objects;
using NetworkUtils.Socket;
using System;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IEndpoint
    {
        ISocket Socket { get; }
        IConnection Connection { get; }

        event EventHandler<DisconnectedArgs> Disconnected;

        Task<bool> Send(byte[] data);
        bool Connected { get; }

        Task Disconnect();
    }
}