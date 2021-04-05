using System;

namespace GameServer
{
    interface ISocket
    {
        event EventHandler Disconnected;
        event EventHandler<byte[]> MessageReceived;

        void Connect(string ip, int port);
        void Disconnect();

        void Send(byte[] data);
    }
}
