using GameServerData;
using MessageUtilities;
using System;

namespace GameServer.Implementation
{
    public interface IConnectionHandler
    {
        event Action<IEndpoint, Message> MessageReceived;
        event Action<IEndpoint> EndpointConnected;
        event Action<IEndpoint> EndpointDisconnected;

        void Start(int port);
        void Close();
    }
}
