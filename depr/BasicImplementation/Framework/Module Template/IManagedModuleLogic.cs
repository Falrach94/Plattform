using GameServer;

namespace ServerImplementation.Modules
{
    public interface IManagedModuleLogic<TData> : IModuleLogic
    {
        new TData Data { get; set; }
        string LogName { set; }
    }
}
