using LogModules.Interface;
using ServerImplementation;
using ServerImplementation.Modules;

namespace LogModule
{
    public class LogModule : Module<LogModuleLogic, LogModuleData, DefaultMessageHandler<LogModuleData, LogModuleLogic>>, ILogModule
    {
        public ILogManager LogManager => Logic;
        public LogModule() : base("Log Module", "log")
        {
        }
    }
}
