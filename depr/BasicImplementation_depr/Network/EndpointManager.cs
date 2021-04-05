using DiagnosticsModule;
using DiagnosticsModule.Interface;
using LogUtils;
using NetworkUtils.Socket;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class EndpointManager : IEndpointManager, IDiagnosticsControl, IDiagnosticsDataInterface
    {
        #region logger
        static private readonly Logger log = Logging.LogManager.GetLogger("Network");
        #endregion

        #region properties

        public int Port { get; private set; }
        public IMessageParser MessageParser { get; set; }
        public IConnectionHandler ConnectionHandler { get; set; }

        public IDiagnosticsDataInterface Diagnostics => this;

        #endregion

        #region fields

        private bool _running;
        private System.Net.Sockets.TcpListener _tcpListener;
        private TaskCompletionSource<byte> _stopFinished;

        #endregion

        #region functions 

        private void ListenForClients()
        {
            _tcpListener.BeginAcceptTcpClient(ClientConnectedCallback, null);
            log.Trace("ready for new connection request");
        }

        private void ClientConnectedCallback(IAsyncResult ar)
        {
            if (!_running)
            {
                //listening for connections stopped
                _stopFinished.SetResult(1);
                return;
            }

            var tcpClient = _tcpListener.EndAcceptTcpClient(ar);
            log.Trace("incoming connection request");

            //prepare for next connect
            ListenForClients();

            //create socket and endpoint for new connection
            var socket = new Socket();
            var endpoint = new Endpoint(socket);
            socket.Log = Logging.LogManager.GetLogger("Socket " + endpoint.Connection.Id);
            socket.SetTcpClient(tcpClient);
            log.Debug("connection request "+ endpoint.Connection);

            //start endpoint message receiver
            endpoint.ReceiveThread = new Thread(() =>
            {
                ReceiveHandler(endpoint);
            });
            endpoint.ReceiveThread.Start();

            endpoint.Disconnected += Endpoint_Disconnected;

            _ = ConnectionHandler.ProcessEndpoint(endpoint.Connection, EndpointChangedEventArgs.NewConnection(endpoint));
            
        }

        private void ReceiveHandler(Endpoint endpoint)
        {
            var stream = endpoint.Socket.Stream;
            while (!endpoint.CancelSource.IsCancellationRequested)
            {
                long lastLen = 0;
                long nextPos = 0;
                try
                {
                    lock (stream)
                    {
                        while (stream.Length == lastLen)
                        {
                            Monitor.Wait(stream);
                        }
                        stream.Position = nextPos;

                        try
                        {
                            while (stream.Length != stream.Position)
                            {
                                var msg = MessageParser.ParseMessage(stream);
                                _ = MessageParser.HandleMessage(endpoint.Connection, msg);
                                nextPos = stream.Position;
                            }
                            nextPos = 0;
                            lastLen = 0;
                            stream.SetLength(0);
                        }
                        catch(SerializationException)
                        {
                            lastLen = stream.Length;
                        }
                    }
                }
                catch(ObjectDisposedException)
                {
                    endpoint.CancelSource.Cancel();
                }
                catch(Exception ex)
                {
                    //TODO
                    log.Error("Unexpected error while parsing message!\n" +ex.ToString());
                }
            }
        }

        #endregion
        #region event handler
        private void Endpoint_Disconnected(object sender, DisconnectedArgs e)
        {
            log.Debug("endpoint disconnected " + ((IEndpoint)sender).Connection);

            ConnectionHandler.ProcessEndpoint(((IEndpoint)sender).Connection, EndpointChangedEventArgs.Disconnect((IEndpoint)sender, e)).Wait();
        }
        #endregion

        #region public functions

        public void StartListeningForConnections(int port)
        {
            log.Info("start listening for connections (port: " + port + ")");

            Port = port;

            _tcpListener = new System.Net.Sockets.TcpListener(IPAddress.Any, Port);

            _running = true;
            _tcpListener.Start();

            ListenForClients();

        }

        public async Task StopListeningForConnections()
        {
            _stopFinished = new TaskCompletionSource<byte>();
            _running = false;
            log.Debug("stop listening for connections");
            _tcpListener.Stop();
            await _stopFinished.Task;

        }

        public string GetDetailedState()
        {
            string output = "";

            output += DiagnosticsUtils.Value("port", Port, true);
            output += DiagnosticsUtils.Value("listening for connections", _running);

            return output;
        }

        #endregion
    }
}
