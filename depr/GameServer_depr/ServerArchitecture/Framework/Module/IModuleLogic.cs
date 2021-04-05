using GameServer.Data_Objects;
using GameServerData;
using MessageUtilities;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IModuleLogic
    {
        IModuleData Data { get; }
        IMessenger Messenger { get; set; }
        IServer Server { get; set; }

        Task Broadcast(Message msg);

        void Initialize();
        Task ShutDown();

        Task Reset();

        Task HandleDisconnect(IConnection connection);
        Task SynchronizeConnection(IConnection connection);
    }
}