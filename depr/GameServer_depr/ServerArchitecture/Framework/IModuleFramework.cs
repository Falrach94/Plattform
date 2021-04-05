using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IModuleFramework
    {
        IMessenger Messenger { get; set; }
        IServer Server { get; set; }

        IReadOnlyCollection<IModule> Modules { get; }


        void AddModule(IModule module);
        Task InitializeModules();
        void LoadState();
        void SaveState();
        Task ShutDown();
        Task Reset();
    }
}