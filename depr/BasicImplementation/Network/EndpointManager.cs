using DiagnosticsModule;
using DiagnosticsModule.Interface;
using LogUtils;
using NetworkUtils.Socket;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GameServer
{
    public class EndpointManager : IEndpointManager, IDiagnosticsControl, IDiagnosticsDataInterface
    {
        #region logger
        static private readonly Logger log = Logging.LogManager.GetLogger("Network");
        #endregion

        #region properties

        public int Port { get; private set; }
        public IMessageHandler MessageParser { get; set; }
        public IConnectionManager ConnectionHandler { get; set; }

        public IDiagnosticsDataInterface Diagnostics => this;

        #endregion

        #region fields

        private bool _running;
        private System.Net.Sockets.TcpListener _tcpListener;
        private TaskCompletionSource _stopFinished;
      //  private ActionBlock<byte[]> _receiveHandlerBlock;

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
                _stopFinished.SetResult();
                return;
            }

            var tcpClient = _tcpListener.EndAcceptTcpClient(ar);
            log.Trace("incoming connection request");

            //prepare for next connect
            ListenForClients();

            //create socket and endpoint for new connection
            var socket = new PacketSocket(tcpClient);
            var endpoint = new Endpoint(socket);

            log.Debug("connection request "+ endpoint.Connection);

            var actionBlock = new ActionBlock<Tuple<IEndpoint, byte[]>>(ReceiveHandler);
            endpoint.MessageBlock.LinkTo(actionBlock);


            endpoint.Disconnected += Endpoint_Disconnected;

            _ = ConnectionHandler.ProcessEndpoint(endpoint.Connection, EndpointChangedEventArgs.NewConnection(endpoint));
            
        }

        private async Task ReceiveHandler(Tuple<IEndpoint, byte[]> msgData)
        {

            try
            {
                var msg = await MessageParser.ParseMessage(msgData.Item2);
                _ = MessageParser.HandleMessage(msgData.Item1.Connection, msg);
            }
            catch (Exception ex)
            {
                //TODO
                log.Error("Unexpected error while parsing message!\n" + ex.ToString());
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
            _stopFinished = new TaskCompletionSource();
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
