using MessageUtils.Messenger;
using ServerKernel.Connections.Manager;
using ServerKernel.Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Protocol
{
    public class ConnectionProtocol : IConnectionProtocolHandler
    {
        public BroadcastMessenger Messenger { get; set; } 


        public PatternUtils.Version Version { get; }

        public ConnectionProtocol()
        {
            Version = PatternUtils.Version.Create(1, 0);
        }

        public Task ClosingConnectionAsync(Connection connection, DisconnectReason reason, string message)
        {
            return Task.CompletedTask;
        }

        public Task DisconnectedConnectionAsync(Connection connection)
        {
            return Task.CompletedTask;
        }

        public Task EstablishedConnectionAsync(Connection connection)
        {
            return Task.CompletedTask;
        }

        public Task NewConnectionAsync(Connection connection)
        {
            return Task.CompletedTask;
        }

        public Task SynchronizingConnectionAsync(Connection connection)
        {
            return Task.CompletedTask;
        }
    }
}
