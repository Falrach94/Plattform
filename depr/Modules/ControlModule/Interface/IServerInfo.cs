using GameServer;
using GameServer.Data_Objects;
using System.Collections.Generic;

namespace ControlModule.Logic
{
    public interface IServerInfo
    {
        IReadOnlyCollection<IModule> Modules { get; }
        IReadOnlyCollection<IConnection> Connections { get; }
    }
}
