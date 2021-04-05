using ServerImplementation.ControlModule;

namespace GameServer
{
    public class ControlModule : IModule
    {
        public ControlMessageHandler MessageHandler { get; }

        public ControlLogic Logic { get; }

        public ControlData Data { get; }

        public string Name { get; } = "Control";

        IModuleLogic IModule.Logic => Logic;

        IModuleMessageHandler IModule.MessageHandler => MessageHandler;

        IModuleData IModule.Data => Data;

        private static ControlModule _instance;

        public IServer Server
        {
            set
            {
                Data.Server = value;
            }
            get => Data.Server;
        }

        private ControlModule()
        {
            Data = new ControlData();
            Logic = new ControlLogic(Data);
            MessageHandler = new ControlMessageHandler(Data, Logic);
        }

        public static ControlModule GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ControlModule();
            }
            return _instance;
        }

        public void GreetModule(IModule module)
        {
            //  throw new NotImplementedException();
        }
    }
}