using GameServer;
using GameServer.Data_Objects;
using LogUtils;
using ServerImplementation.Framework.Module_Template;
using SyncUtils;

namespace ServerImplementation.Modules
{
    public abstract class ModuleData : IModuleData
    {
        protected Logger log = Logging.LogManager.GetLogger("Data");

        public StateSynchronizer Synchronizer { get; } = new StateSynchronizer();

        public virtual bool IsSynchronized(IConnection con)
        {
            return false;
        }

        public virtual void Load()
        {
        }

        public virtual void Save()
        {
        }
    }
}
