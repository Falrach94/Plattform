using GameServer;
using GameServer.Data_Objects;
using LogUtils;
using MessageUtilities;
using SyncUtils;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerImplementation.Modules
{
    public abstract class ModuleLogic<TData> : IManagedModuleLogic<TData>
        where TData : IModuleData
    {

        protected Logger log;

        //protected readonly AsyncMutex Mutex = new AsyncMutex();
        protected readonly SemaphoreLock Mutex = new();

        public string LogName { set => log = Logging.LogManager.GetLogger(value); }

        public TData Data { get; set; }
        public IMessenger Messenger { get; set; }
        public IServer Server { get; set; }

        IModuleData IModuleLogic.Data => Data;

        public async Task Broadcast(Message msg)
        {
            await Messenger.BroadcastMessage(Server.ConnectionStorage.Connections.Where(c => Data.IsSynchronized(c)).ToArray(), msg);          
        }


        public virtual Task HandleDisconnect(IConnection connection)
        {
            return Task.CompletedTask;
        }

        public virtual void Initialize()
        {
        }

        public virtual Task ShutDown()
        {
            Reset();
            return Task.CompletedTask;
        }

        public virtual Task SynchronizeConnection(IConnection connection)
        {
            return Task.CompletedTask;
        }

        public abstract Task Reset();

        /*public virtual Task Reset()
        {
            return Task.CompletedTask;
        }*/
    }
}
