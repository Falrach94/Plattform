namespace GameServer
{
    public interface IModule
    {
        IServer Server { get; set; }

        IModuleLogic Logic { get; }
        IModuleMessageHandler MessageHandler { get; }
        IModuleData Data { get; }

        string Name { get; }

        void GreetModule(IModule module);
    }
}