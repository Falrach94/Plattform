using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer;
using GameServer.Data_Objects;
using ServerImplementation.Exceptions;
using ServerImplementation.Modules;
using ServerImplementation.Utils;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ServerImplementation.ConnectionHandler
{
    public class ConnectionData : ModuleData, IConnectionStorage, IDiagnosticsDataInterface
    {
        public IReadOnlyCollection<IConnection> Connections => _connections.Values;

        IReadOnlyCollection<IConnection> IConnectionStorage.Connections => Connections;

        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();


        internal void RemoveConnection(Connection connection)
        {
            _connections.Remove(connection.Id);
            log.Debug("removed connection " + connection);
        }
        internal void AddConnection(Connection connection)
        {
            _connections.Add(connection.Id, connection);
            log.Debug("added connection " + connection);
        }

        public IConnection GetConnectionById(int id)
        {
            return _connections[id];
        }

        public bool ConnectionExists(int id)
        {
            return _connections.ContainsKey(id);
        }

        internal async Task SetConnectionState(Connection connection, ConnectionState state)
        {
            log.Trace("try to change connection " + connection + " state to " + state);

            using (var token = await connection.StateMutex.LockAsync(Const.DEADLOCK_TIMEOUT))
            {
                if (connection.State == ConnectionState.Disconnected
                && state != ConnectionState.Disconnected)
                {
                    throw new EndpointDisconnectedException();
                }

                connection.State = state;

                if (state == ConnectionState.Synchronizing)
                {
                    connection.StateMutex.PulseAll(token);
                }
            }

            log.Debug("connection " + connection + " state changed to " + state);
        }


        public string GetDetailedState()
        {
            string output = string.Empty;



            output += DiagnosticsUtils.ListString("Connections", _connections);

            return output;
        }

        IConnection IConnectionStorage.GetConnectionById(int id)
        {
            return _connections[id];
        }
    }
}
