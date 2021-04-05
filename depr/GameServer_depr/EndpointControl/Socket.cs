using NLog;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace GameServer
{
    public class Socket : ISocket
    {
        public event EventHandler Disconnected;
        public event EventHandler<byte[]> MessageReceived;

        private const int BUFFER_SIZE = 4096;

        private TcpClient _socket;
        private byte[] _receiveBuffer = new byte[BUFFER_SIZE];

        public NetworkStream Stream { get; set; }

        public void SetTcpClient(TcpClient socket)
        {
            log.TraceEntry();

            _socket = socket;

            _socket.ReceiveBufferSize = BUFFER_SIZE;
            _socket.SendBufferSize = BUFFER_SIZE;

            Stream = _socket.GetStream();

            StartReceive();

            log.TraceExit();
        }
        public void Connect(string ip, int port)
        {
            log.TraceEntry();
            SetTcpClient(new TcpClient(ip, port));
            log.TraceExit();
        }

        private void StartReceive()
        {
            log.TraceEntry();
            Stream.BeginRead(_receiveBuffer, 0, BUFFER_SIZE, ReceiveCallback, null);
            log.TraceExit();
        }


        public void Disconnect()
        {
            _socket.Close();
            _socket = null;
            Stream.Close();
            Stream = null;
            Disconnected?.Invoke(this, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int length = Stream.EndRead(ar);
            if (length == 0)
            {
                Disconnect();
            }
            else
            {
                byte[] data = new byte[length];

                Array.Copy(_receiveBuffer, data, length);

                StartReceive();

                MessageReceived?.Invoke(this, data);
            }
        }

        public void Send(byte[] data)
        {
            Stream.Write(data, 0, data.Length);
        }
    }
}
