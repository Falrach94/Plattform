using GameServer.Data_Objects;
using System.Collections.Generic;

namespace GameServer
{
    public interface IConnectionStorage
    {
        IReadOnlyCollection<IConnection> Connections { get; }
        IConnection GetConnectionById(int id);
        bool ConnectionExists(int id);
    }
}