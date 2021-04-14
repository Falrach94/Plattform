using MessageUtilities;
using MessageUtils.Messenger;
using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerTests.Mocks
{
    public class MockEndpoint : IEndpoint, IMessageSender
    {
        private MockSocket _socket = new MockSocket();
        public ISocket Socket => _socket;

        public object ConnectionData { get; set; }

        public Action<Message> MessageSent { get; set; }

//        public void ReceiveMsg(object )

        public MockEndpoint()
        {
            _socket.IsConnected = true;
            _socket.Disconnected += _socket_Disconnected;
        }

        private void _socket_Disconnected(object sender, DisconnectedArgs e)
        {
            Disconnected?.Invoke(this, e);
        }
        private BufferBlock<RawMessage> _rawMsgBlock = new();

        public ISourceBlock<RawMessage> RawMessageBlock => _rawMsgBlock;

        public event EventHandler<DisconnectedArgs> Disconnected;

        public void Disconnect(bool remote)
        {
            if (remote)
            {
                _socket.DisconnectRemote();
            }
            else
            {
                _socket.DisconnectAsync().Wait();
            }
        }

        public Task<bool> SendAsync(byte[] msg)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SendAsync(Message msg)
        {
            _rawMsgBlock.Post(new RawMessage(this, msg.Serialize()));
            return Task.FromResult(true);
        }
    }

}
