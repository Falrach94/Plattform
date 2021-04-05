using LogUtils;
using ServerImplementation.ConnectionModule;

namespace GameServer
{
    public class BasicServer : Server
    {
        public Messenger Messenger { get; } = new Messenger();

        public EndpointManager Network { get; } = new EndpointManager();

        public FrameworkManager ModuleManager { get; } = new FrameworkManager();

        public ConnectionModule Connections { get; } = new ConnectionModule();

        public override int Port => Network.Port;

        public BasicServer()
        {

            //ServerLogTarget.Out = Console.Out;

            Initialize(Connections.Logic,
                        Messenger,
                        Connections.Data,
                        Network,
                        Messenger,
                        ModuleManager,
                        ModuleManager,
                        Logging.LogManager.GetInstance());

            LoadModule(Connections);

        }
    }
}