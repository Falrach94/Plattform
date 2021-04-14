using MessageUtils;
using ServerKernel.Connections.Manager;
using ServerKernel.Data_Objects;
using ServerKernel.Messaging;
using System;
using System.Threading.Tasks;

namespace ServerTests.Mocks
{
    public class MockProtocol : IConnectionProtocolHandler, IMessageErrorProtocol
    {
        public PatternUtils.Version Version => PatternUtils.Version.Create(1);

        public Func<Connection, Task> SynchronizingConnection { get; set; }
        public Func<Connection, DisconnectReason, string, Task> ClosingConnection { get; set; }
        public Func<Connection, Task> DisconnectedConnection { get; set; }
        public Func<Connection, Task> EstablishedConnection { get; set; }
        public Func<Connection, Task> NewConnection { get; set; }
        public Func<MessageProcessingError, Task> MessageErrorHandler { get; set; }

        public bool Working { get; set; } = true;
        public int DelayTime { get; set; } = 100;

        private void Workload()
        {
            if (!Working)
            {
                throw new Exception("mock exception");
            }
            Task.Delay(DelayTime).Wait();
        }

        public Task ClosingConnectionAsync(Connection connection, DisconnectReason reason, string message)
        {
            Workload();
            if (ClosingConnection == null) return Task.CompletedTask;

            return ClosingConnection?.Invoke(connection, reason, message);
        }

        public Task DisconnectedConnectionAsync(Connection connection)
        {
            Workload();
            if (DisconnectedConnection == null) return Task.CompletedTask;
            return DisconnectedConnection?.Invoke(connection);
        }

        public Task EstablishedConnectionAsync(Connection connection)
        {
            Workload();
            if (EstablishedConnection == null) return Task.CompletedTask;
            return EstablishedConnection?.Invoke(connection);
        }

        public Task NewConnectionAsync(Connection connection)
        {
            Workload();
            if (NewConnection == null) return Task.CompletedTask;
            return NewConnection?.Invoke(connection);
        }

        public Task SynchronizingConnectionAsync(Connection connection)
        {
            Workload();
            if (SynchronizingConnection == null) return Task.CompletedTask;
            return SynchronizingConnection?.Invoke(connection);
        }

        public Task HandleMessageErrorAsync(MessageProcessingError error)
        {
            if (MessageErrorHandler == null) return Task.CompletedTask;
            return MessageErrorHandler?.Invoke(error);
        }
    }

}
