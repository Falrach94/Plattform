using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using PatternUtils;
using PatternUtils.Ids;
using ServerKernel.Data_Objects;
using ServerUtils;
using ServerUtils.Endpoint_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerKernel.Connections.Manager
{
    /// <summary>
    /// Consumes endpoint state changes and handles connection creation/destruction accordingly.
    /// </summary>
    public class ConnectionManager : IConnectionControl, IUnsubscribeable<IConnectionProcessor>
    {
        public IEndpointObservable Endpoints
        {
            get => _endpoints;
            set
            {
                if (_endpoints != null)
                {
                    _endpoints.EndpointConnectedHandler = null;
                    _endpoints.EndpointDisconnectedHandler = null;
                }
                _endpoints = value;
                if (_endpoints != null)
                {
                    _endpoints.EndpointConnectedHandler = EndpointConnected;
                    _endpoints.EndpointDisconnectedHandler = EndpointDisconnected;
                }
            }
        }
        public IEndpointControl EndpointControl { get; set; }
        public IConnectionProtocolHandler ProtocolHandler { get; set; }
        public List<Connection> Connections => Endpoints.ConnectedEndpoints.Select(ep => (Connection)ep.ConnectionData).ToList();

        private readonly SemaphoreSlim _sem = new(1,1);

        private readonly List<Connection> _connections = new();
        private readonly List<IConnectionProcessor> _connectionProcessors = new();
        private readonly IdPool _idPool = new();


        private IEndpointObservable _endpoints;



        private async Task EndpointConnected(IEndpoint ep)
        {
            var connection = new Connection();
            ep.ConnectionData = connection;

            connection.Endpoint = ep;
            connection.IdReturner = _idPool.GetNextId(out int id);
            connection.Id = id;

            await SetAndHandleConnectionStateAsync(connection, ConnectionState.Connecting);

            await _sem.WaitAsync();
            _connections.Add(connection);
            _sem.Release();
        }
        private async Task EndpointDisconnected(IEndpoint ep, bool remoteDisconnect)
        {
            var connection = (Connection)ep.ConnectionData;


            await _sem.WaitAsync();
            if (!_connections.Remove(connection))
            {
                throw new ArgumentException("Connection not found!");
            }
            _sem.Release();

            await SetAndHandleConnectionStateAsync(connection, ConnectionState.Disconnected);
            connection.Dispose();
        }

        protected async Task SetAndHandleConnectionStateAsync(Connection connection, ConnectionState state)
        {
            connection.State = state;
            switch (state)
            {
                case ConnectionState.Connecting:
                    await ProtocolHandler.NewConnectionAsync(connection);
                    await SetAndHandleConnectionStateAsync(connection, ConnectionState.Synchronizing);
                    break;
                case ConnectionState.Synchronizing:
                    await SynchronizeConnection(connection);
                    break;
                case ConnectionState.Established:
                    await ProtocolHandler.EstablishedConnectionAsync(connection);
                    break;
                case ConnectionState.Disconnected:
                    await DisconnectConnection(connection);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task DisconnectConnection(Connection connection)
        {
            var tasks = new List<Task>();
            foreach (var processor in _connectionProcessors)
            {
                tasks.Add(processor.Disconnect(connection));
            }
            await Task.WhenAll(tasks);
            await ProtocolHandler.DisconnectedConnectionAsync(connection);
        }

        private async Task SynchronizeConnection(Connection connection)
        {
            await ProtocolHandler.SynchronizingConnectionAsync(connection);

            var tasks = new List<Task>();
            foreach (var processor in _connectionProcessors)
            {
                tasks.Add(processor.Synchronize(connection));
            }
            await Task.WhenAll(tasks);

            await SetAndHandleConnectionStateAsync(connection, ConnectionState.Established);
        }

        public async Task CloseAllConnectionsAsync()
        {
            List<Task> tasks = new();

            var cons = new List<Connection>(_connections);

            foreach (var c in cons)
            {
                tasks.Add(CloseConnectionAsync(c, DisconnectReason.ServerClosed, "server reset"));
            }

            await Task.WhenAll(tasks);

        }

        public async Task CloseConnectionAsync(Connection connection, DisconnectReason reason, string message)
        {
            await ProtocolHandler.ClosingConnectionAsync(connection, reason, message);
            await EndpointControl.DisconnectEndpointAsync(connection.Endpoint);
        }

        public IDisposable RegisterConnectionProcessor(IConnectionProcessor processor)
        {
            _connectionProcessors.Add(processor);
            return new Unsubscriber<IConnectionProcessor>(processor, this);
        }

        public void Unsubscribe(IConnectionProcessor subscribedObject)
        {
            _connectionProcessors.Remove(subscribedObject);
        }
    }
}               