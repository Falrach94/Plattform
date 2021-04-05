using ServerKernel.Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Connections.Manager
{
    public interface IConnectionProtocolHandler
    {
        PatternUtils.Version Version { get; }

        Task ClosingConnectionAsync(Connection connection, DisconnectReason reason, string message);
        Task DisconnectedConnectionAsync(Connection connection);
        Task EstablishedConnectionAsync(Connection connection);
        Task SynchronizingConnectionAsync(Connection connection);
        Task NewConnectionAsync(Connection connection);
    }
}
