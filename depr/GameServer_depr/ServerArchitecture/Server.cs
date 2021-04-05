
using LogUtils;
using SyncUtils;
using System;
using System.Threading.Tasks;

namespace GameServer
{

    public class Server : IServer, IDisposable
    {
        protected Logger log;
        public bool Initialized { get; private set; }
        public bool Running { get; private set; }

        public AsyncPulseSource RunningMutex { get; } = new AsyncPulseSource();

        public IModuleFramework Framework { get; private set; }
        public IConnectionStorage ConnectionStorage { get; private set; }
        public IConnectionHandler ConnectionHandler { get; private set; }

        public LogManager LogManager { get; private set; }

        public virtual int Port { get; }

        private IEndpointManager _network;

        protected void Initialize(IConnectionHandler connectionHandler,
                                    IMessageParser parser,
                                    IConnectionStorage connectionStorage,
                                    IEndpointManager endpointManager,
                                    IMessenger broadcaster,
                                    IModuleFramework moduleFramework,
                                    IModuleControl moduleControl,
                                    LogManager logManager)
        {

            if (Initialized)
            {
                throw new Exception("Server is already initialized!");
            }

            LogManager = logManager;
            log = LogManager.GetLogger("Server");

            _network = endpointManager;
            Framework = moduleFramework;

            Framework.Messenger = broadcaster;
            Framework.Server = this;

            ConnectionHandler = connectionHandler;

            ConnectionStorage = connectionStorage;

            _network.ConnectionHandler = connectionHandler;
            _network.MessageParser = parser;

            connectionHandler.Modules = moduleControl;
            connectionHandler.Messenger = broadcaster;

            parser.Modules = moduleControl;
            parser.ConnectionHandler = connectionHandler;

            broadcaster.Connections = connectionStorage;

            Initialized = true;

        }

        public async virtual Task Start(int port)
        {
            log.Debug("start server on port " + port);

            if (!Initialized)
            {
                throw new Exception("Server has not been initialized yet!");
            }

            await Framework.InitializeModules();

            _network.StartListeningForConnections(port);

            using (var l = await RunningMutex.Lock())
            {
                Running = true;
                RunningMutex.PulseAll(l);
            }
            log.Info("server running");

        }

        public async virtual Task ShutDown()
        {
            log.Debug("shutting down...");
            
            await _network.StopListeningForConnections();

            await Framework.ShutDown();
            using (var l = await RunningMutex.Lock())
            {
                Running = false;
                RunningMutex.PulseAll(l);
            }

            log.Info("shut down complete");
        }


        public void LoadModule(IModule module)
        {
            if (Running)
            {
                throw new Exception("Modules can not be added to running server!");
            }
            Framework.AddModule(module);

        }

        public void Dispose()
        {
            _ = ShutDown();
        }

        public async Task Reset(int port)
        {
            log.Debug("resetting...");

            Running = false;

            await _network.StopListeningForConnections();

            await Framework.Reset();

            using (var l = await RunningMutex.Lock())
            {
                Running = true;
                RunningMutex.PulseAll(l);
            }

            _network.StartListeningForConnections(port);

            log.Debug("reset done");
        }
    }
}