using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using GameSettingUtils;
using ServerImplementation.Modules;
using System.Collections.Generic;

namespace GameManagement.LobbyM.Data
{
    public class LobbyModuleData : ModuleData, IDiagnosticsDataInterface
    {
        private readonly Dictionary<int, LobbyConnection> _connectionDic = new Dictionary<int, LobbyConnection>();

        private readonly Dictionary<int, Lobby> _lobbyDic = new Dictionary<int, Lobby>();
        public IReadOnlyDictionary<int, Lobby> Lobbies => _lobbyDic;
        public IReadOnlyDictionary<int, LobbyConnection> Connections => _connectionDic;

        public string GetDetailedState()
        {
            string output = "";

            output += DiagnosticsUtils.ListString("synchronized connections", _connectionDic, true);

            output += DiagnosticsUtils.ListString("lobbies", _lobbyDic);

            return output;
        }

        public void AddConnection(IConnection connection)
        {
            _connectionDic.Add(connection.Id, new LobbyConnection(connection));
        }
        public void RemoveConnection(IConnection connection)
        {
            _connectionDic.Remove(connection.Id);
        }

        internal void SetSetting(IGameSetting setting, object value)
        {
            setting.Value = value;
            log.Debug("set setting (" + setting.Name + ", " + value + ")");
        }

        public override bool IsSynchronized(IConnection con)
        {
            return _connectionDic.ContainsKey(con.Id);
        }

        public void AddLobby(Lobby lobby)
        {
            _lobbyDic.Add(lobby.Id, lobby);
            log.Debug("lobby " + lobby + " added");
        }

        public void RemoveLobby(Lobby lobby)
        {
            _lobbyDic.Remove(lobby.Id);
            log.Debug("lobby " + lobby + " removed");
        }

        internal void SetReady(LobbyConnection connection, bool v)
        {
            connection.Ready = v;
            log.Debug("connection " + connection.Connection.Id + " ready changed to " + v);
        }

        internal void AddLobbyConnection(Lobby l, LobbyConnection c)
        {
            l.Connections.Add(c.Connection.Id, c);
            c.Lobby = l;
            log.Debug("added connection " + c + " to lobby " + l);    
        }
    }
}