using GameServer;
using GameManagement;
using GameManagement.GameStorageModule;
using System.Threading.Tasks;
using GameManagement.LobbyM.Data;

namespace TestProject
{
    public class TestServer : BasicServer
    {
        public GameServer.PlayerModule PlayerModule { get; } = GameServer.PlayerModule.GetInstance();
        public GameServer.ControlModule ControlModule { get; } = GameServer.ControlModule.GetInstance();

        public ChatModuleM.ChatModule ChatModule { get; } = new ChatModuleM.ChatModule();
        public DummyClientModule.DummyClientModule DummyClientModule { get; } = new DummyClientModule.DummyClientModule();

        public TextInterfaceModule.TextInterfaceModule TextInterface { get; }

        public LogModule.LogModule LogModule { get; } = new LogModule.LogModule();
        public DiagnosticsModule.DiagnosticsModule DiagnosticsModule { get; } = new DiagnosticsModule.DiagnosticsModule();

        public GameStorageModule GameStorageModule { get; } = new GameStorageModule();
        public GameManagement.LobbyModule LobbyModule { get; }

        public GameManagement.Module.GameManagementModule GameManagementModule { get; } = new GameManagement.Module.GameManagementModule();
        public TestServer()
        {
            ControlModule.Server = this;

            TextInterface = new TextInterfaceModule.TextInterfaceModule(ControlModule.Logic, ControlModule.Logic);
            LobbyModule = new GameManagement.LobbyModule(GameStorageModule.Logic,
                                                         ChatModule.Logic,
                                                         PlayerModule.PlayerStorage, GameManagementModule.Logic);

            DiagnosticsModule.Logic.RegisterModule("Network", Network);

            LoadModule(LogModule);
            LoadModule(DiagnosticsModule);
            LoadModule(TextInterface);
            LoadModule(PlayerModule);
            LoadModule(ChatModule);
            LoadModule(DummyClientModule);
            LoadModule(LobbyModule);
            LoadModule(GameStorageModule);
            LoadModule(GameManagementModule);

            GameStorageModule.Logic.LoadDll("DummyGame.dll");

        }

        public override Task Start(int port)
        {
            LogModule.Logic.StartLogThread();
            return base.Start(port);
        }

        public async override Task ShutDown()
        {
            await base.ShutDown();
            LogModule.Logic.StopLogThread();
        }
    }
}
