using GameServer;
using LogUtils;

namespace ServerImplementation.Modules
{
    public abstract class Module<TLogic, TData, TMessageHandler> : IModule
        where TData : IModuleData, new()
        where TLogic : IManagedModuleLogic<TData>, new()
        where TMessageHandler : IManagedModuleMessageHandler<TLogic, TData>, new()
    {
        protected readonly Logger log;

        public IServer Server { get; set; }

        public TMessageHandler MessageHandler { get; } = new TMessageHandler();
        public TLogic Logic { get; } = new TLogic();
        public TData Data { get; } = new TData();

        IModuleLogic IModule.Logic => Logic;
        IModuleMessageHandler IModule.MessageHandler => MessageHandler;
        IModuleData IModule.Data => Data;
        public string Name { get; }

        public Module(string name, string logName)
        {
            log = Logging.LogManager.GetLogger(logName);

            Name = name;

            Logic.Data = Data;
            Logic.LogName = logName;

            MessageHandler.Data = Data;
            MessageHandler.LogName = logName;
            MessageHandler.Logic = Logic;
            MessageHandler.Name = name;
        }

        public virtual void GreetModule(IModule module)
        {
        }
    }
}
