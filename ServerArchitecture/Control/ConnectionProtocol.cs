using ServerKernel.Connections.Manager;
using ServerKernel.Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Control
{
    public class ConnectionProtocol : IConnectionProtocolHandler
    {

        public PatternUtils.Version Version { get; }

        public ConnectionProtocol()
        {
            Version = PatternUtils.Version.Create(1, 0);
        }

        public Task ClosingConnectionAsync(Connection connection, DisconnectReason reason, string message)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectedConnectionAsync(Connection connection)
        {
            throw new NotImplementedException();
        }

        public Task EstablishedConnectionAsync(Connection connection)
        {
            throw new NotImplementedException();
        }

        public Task NewConnectionAsync(Connection connection)
        {
            throw new NotImplementedException();
        }

        public Task SynchronizingConnectionAsync(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
