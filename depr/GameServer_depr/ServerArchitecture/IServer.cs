using LogUtils;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IServer
    {
        IModuleFramework Framework { get; }
        IConnectionHandler ConnectionHandler { get; }
        IConnectionStorage ConnectionStorage { get; }

        int Port { get; }

        LogManager LogManager { get; }

        bool Initialized { get; }
        bool Running { get; }

        Task Reset(int port = -1);

        Task ShutDown();
        Task Start(int port);
        void LoadModule(IModule module);
    }
}