using GameServer;

namespace ServerImplementation.Modules
{
    public interface IManagedModuleMessageHandler<TLogic, TData> : IModuleMessageHandler
    {
        new TLogic Logic { get; set; }
        new TData Data { get; set; }
        string LogName { set; }
    }
}
