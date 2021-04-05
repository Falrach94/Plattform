using GameServer.Data_Objects;
using GameServerData;
using MessageUtilities;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IModuleMessageHandler
    {
        IMessenger Messenger { get; set; }
        IServer Server { get; set; }
        IModuleLogic Logic { get; }
        IModuleData Data { get; }

        string Name { get; set; }

        Task ProcessMessage(IConnection sender, Message msg);
    }
}