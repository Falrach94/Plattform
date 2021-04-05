using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using LobbyModule.GameManagementModule.Data;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameManagement
{
    public class GameManagementData : ModuleData, IDiagnosticsDataInterface
    {
        private Dictionary<int, GameInstance> _activeGamesDic = new Dictionary<int, GameInstance>();
        private Dictionary<int, GameInstance> _gameHistoryDic = new Dictionary<int, GameInstance>();
        private Dictionary<int, GameConnection> _connections = new Dictionary<int, GameConnection>();

        public IReadOnlyDictionary<int, GameInstance> ActiveGameDic => _activeGamesDic;
        public IReadOnlyDictionary<int, GameInstance> GameHistoryDic => _gameHistoryDic;
        public IReadOnlyDictionary<int, GameConnection> Connections => _connections;


        public string GetDetailedState()
        {
            string output = "";
            output += DiagnosticsUtils.ListString("synchronized connections", _connections, true);
            output += DiagnosticsUtils.ListString("active games", _activeGamesDic,true);
            output += DiagnosticsUtils.ListString("game history", _gameHistoryDic);
            return output;
        }

        public void AddActiveGame(GameInstance game)
        {
            _activeGamesDic.Add(game.Id, game);
        }
        public void RemoveActiveGame(GameInstance game)
        {
            _activeGamesDic.Remove(game.Id);
        }
        public void AddHistoricGame(GameInstance game)
        {
            _gameHistoryDic.Add(game.Id, game);
        }
        public void RemoveHistoricGame(GameInstance game)
        {
            _gameHistoryDic.Remove(game.Id);
        }

        internal void AddGameConnection(IConnection connection)
        {
            _connections.Add(connection.Id, new GameConnection(connection));
        }

        internal void RemoveGameConnection(IConnection connection)
        {
            _connections.Remove(connection.Id);
        }
    }
}