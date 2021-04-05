using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using GameManagement.GameStorageModule.Data;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameManagement.GameStorageModule
{
    public class GameStorageData : ModuleData, IDiagnosticsDataInterface
    {
        private readonly Dictionary<int, StorageConnection> _connections = new Dictionary<int, StorageConnection>();

        private readonly Dictionary<int, GameWrapper> _games = new Dictionary<int, GameWrapper>();
        public IReadOnlyDictionary<int, GameWrapper> Games => _games;
        public IReadOnlyDictionary<int, StorageConnection> Connections => _connections;

        public void AddConnection(IConnection connection)
        {
            _connections.Add(connection.Id, new StorageConnection());
        }
        public void RemoveConnection(IConnection connection)
        {
            _connections.Remove(connection.Id);
        }

        public string GetDetailedState()
        {
            string output = "";

            output += DiagnosticsUtils.ListString("Synchronized Connections", _connections, true);

            output += DiagnosticsUtils.CollectionListString("Games", _games, m => m.Factory.Name);

            return output;
        }

        internal void AddModule(IServerGameFactory game)
        {
            if(_games.Values.ToArray().Where(g=>g.Factory.Name == game.Name).Any())
            {
                throw new ArgumentException("Game with name " + game.Name + " already exists!");
            }
            var wrapper = new GameWrapper(game);
            _games.Add(wrapper.Id, wrapper);
        }

        internal void AddGameToConnection(IConnection connection, ICollection<GameWrapper> games)
        {
            foreach (var g in games)
            {
                _connections[connection.Id].SupportedGames.Add(g);
            }
        }
    }
}