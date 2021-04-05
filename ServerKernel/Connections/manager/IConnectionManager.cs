using ServerKernel.Data_Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerKernel.Connections.Manager
{
    /// <summary>
    /// Provides functions for connection management.
    /// </summary>
    public interface IConnectionManager 
    {
        Task CloseConnectionAsync(Connection client, DisconnectReason reason, string message);
        Task CloseAllConnectionsAsync();

        List<Connection> Connections { get; }
    }
}