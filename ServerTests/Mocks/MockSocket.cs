using NetworkUtils.Socket;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerTests.Mocks
{
    public class MockSocket : ISocket
    {
        public BufferBlock<byte[]> IncomingBufferBlock => throw new NotImplementedException();

        public bool IsConnected { get; set; }

        public bool IsReceiving => IsConnected;

        public event EventHandler<DisconnectedArgs> Disconnected;

        public Task<bool> ConnectAsync(string ip, int port)
        {
            IsConnected = true;
            return Task.FromResult(true);
        }

        public void Disconnect(bool remote)
        {
            if (remote)
                DisconnectRemote();
            else
                DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            IsConnected = false;
            Disconnected?.Invoke(this, new DisconnectedArgs(false));
            return Task.CompletedTask;
        }
        public void DisconnectRemote()
        {
            IsConnected = false;
            Disconnected?.Invoke(this, new DisconnectedArgs(true));
        }

        public Task<bool> SendAsync(byte[] data)
        {
            return Task.FromResult(true);
        }
    }

}
