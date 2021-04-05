using NetworkUtils.Socket;
using PatternUtils;
using PatternUtils.Ids;
using ServerKernel.Data_Objects;
using ServerUtils;
using ServerUtils.Endpoint_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ServerKernel.Connections.Manager
{
    /// <summary>
    /// Consumes endpoint state changes and handles connection creation/destruction accordingly.
    /// </summary>
    /// <typeparam name="TStorage"></typeparam>
    public class ConnectionManager : IConnectionManager, IUnsubscribeable<IConnectionProcessor>
    {
        public IEndpointManager EndpointManager 
        { 
            get => _endpointManager;
            set 
            {
                if(value is null)
                {
                    throw new ArgumentNullException();
                }
                if(_endpointManager != null)
                {
                    _endpointManager.EndpointConnectionChanged -= EndpointStateChanged;
                }

                _endpointManager = value;
                _endpointManager.EndpointConnectionChanged += EndpointStateChanged;
            }
        }
        public IConnectionProtocolHandler ProtocolHandler { get; set; }
        public List<Connection> Connections => EndpointManager.ConnectedEndpoints.Select(ep => (Connection)ep.ConnectionData).ToList();


        private readonly List<Connection> _connections = new();
        private readonly List<IConnectionProcessor> _connectionProcessors = new();
        private readonly IdPool _idPool = new();

        
        private IEndpointManager _endpointManager;




        private void EndpointStateChanged(object sender, EndpointChangedEventArgs e)
        {
            
            switch(e.Type)
            {
                case EndpointEventType.Connect:
                    {
                        var connection = new Connection();
                        connection.Endpoint = e.Endpoint;
                        e.Endpoint.ConnectionData = connection;
                    
                        connection.IdReturner = _idPool.GetNextId(out int id);
                        connection.Id = id;

                        SetAndHandleConnectionStateAsync(connection, ConnectionState.Connecting).Wait();
                        _connections.Add(connection);

                        break;
                    }
                case EndpointEventType.Disconnect:
                    {
                        var connection = (Connection)e.Endpoint.ConnectionData;

                        _connections.Remove(connection);
                        SetAndHandleConnectionStateAsync(connection, ConnectionState.Disconnected).Wait();
                        connection.Dispose();

                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
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
            await _endpointManager.DisconnectEndpointAsync(connection.Endpoint);
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