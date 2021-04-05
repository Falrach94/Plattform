using GameServer.Data_Objects;
using GameServerData;
using MessageUtilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IModuleControl
    {
        IReadOnlyCollection<IModule> LoadedModules { get; }

        Task HandleDisconnect(IConnection client);
        Task SynchronizeClient(IConnection client);
        Task DistributeMessage(IConnection sender, Message msg);
    }
}