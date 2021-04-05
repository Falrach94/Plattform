using ChatModule;
using DiagnosticsModule.Interface;
using GameManagement.Interface;
using GameServer.Data_Objects;
using GameManagement.LobbyM.Data;
using PlayerModule.Data;
using PlayerModule.Interface;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextInterfaceModule.Interface;
using TextInterfaceModule.Logic.Parser;
using TextParser.Parser;
using GameManagement.LobbyM.Logic;
using GameManagement.LobbyM;

namespace GameManagement
{
    public class LobbyModule : Module<LobbyLogic, LobbyModuleData, LobbyModuleMessageHandler>, IDiagnosticsControl, IRegisterTextInterface
    {
        public LobbyModule(IGameModuleStorage gameStorage, IChatControl chat, IPlayerStorage players, IGameManagement gameManagement) : base("Lobby", "Lobby Module")
        {
            Logic.ChatControl = chat;
            Logic.GameDic = gameStorage.Games;
            Logic.PlayerStorage = players;
            Logic.GameManagement = gameManagement;
        }

        public IDiagnosticsDataInterface Diagnostics => Data;

        public void RegisterTextInterface(ITextInterfaceControl textInterface)
        {
            textInterface.AddToken("Create", "^create");
            textInterface.AddToken("Close", "^close");
            textInterface.AddToken("Lobby", "^lobby");
            textInterface.AddToken("Join", "^join");
            textInterface.AddToken("Leave", "^leave");
            textInterface.AddToken("Player", "^player");
            textInterface.AddToken("Add", "^add");
            textInterface.AddToken("Remove", "^remove");
            textInterface.AddToken("From", "^from");
            textInterface.AddToken("To", "^to");

            textInterface.AddRule(ParserSequence.Start().Token("Lobby").Token("Create").String("Name").Number("GameId").Number("Host").End(),
                (c) => Command_CreateLobby(c.Get<int>("Host"), c.Get<int>("GameId"), c.Get<string>("Name")));
            textInterface.AddRule(ParserSequence.Start().Token("Lobby").Number("LobbyId").Token("Join").Number("ConnectionId").End(),
                (c) => Command_JoinLobby(c.Get<int>("ConnectionId"), c.Get<int>("LobbyId")));
            textInterface.AddRule(ParserSequence.Start().Token("Lobby").Number("LobbyId").Token("Close").End(),
                (c) => Command_CloseLobby(c.Get<int>("LobbyId")));
            textInterface.AddRule(ParserSequence.Start().Token("Lobby").Number("LobbyId").Token("Leave").End(),
                (c) => Command_LeaveLobby(c.Get<int>("LobbyId")));
            textInterface.AddRule(ParserSequence.Start().Token("Lobby").Token("Player").Token("Remove").Number("PlayerId").End(),
                 (c) => Command_RemovePlayerFromLobby(c.Get<int>("PlayerId")));
            textInterface.AddRule(ParserSequence.Start().Token("Lobby").Token("Player").Token("Add").Number("PlayerId").End(),
                 (c) => Command_AddPlayerToLobby(c.Get<int>("PlayerId")));

        }

        private string Command_JoinLobby(int connectionId, int lobbyId)
        {
            var connection = GetLobbyConnection(connectionId);
            var lobby = GetLobby(lobbyId);
            Logic.JoinLobby(connection, lobby).Wait();
            return "Connection " + connectionId + " joined lobby " + lobby.Name;
        }

        private string Command_AddPlayerToLobby(int id)
        {
            var player = GetPlayer(id);
            var connection = GetLobbyConnection(player.Connection.Id);
            var lobby = connection.Lobby;
            if (lobby == null)
            {
                throw new ArgumentException("Connection is not member in any lobby.");
            }
            if (lobby.Players.Contains(player))
            {
                throw new ArgumentException("Player is already member of this lobby.");
            }
            Logic.AddPlayerToLobby(new []{ player}, lobby).Wait();

            return "Player '" + player.Name + "' added to lobby '" + lobby.Name + "'!";
        }

        private Player GetPlayer(int id)
        {
            if(!Logic.PlayerStorage.PlayerExists(id))
            {
                throw new ArgumentException("No player with this id found!");
            }
            return Logic.PlayerStorage.GetPlayerById(id);
        }

        private string Command_RemovePlayerFromLobby(int id)
        {
            var player = GetPlayer(id);
            var connection = GetLobbyConnection(player.Connection.Id);
            var lobby = connection.Lobby;
            if (lobby == null)
            {
                throw new ArgumentException("Connection is not member in any lobby.");
            }
            if (!lobby.Players.Contains(player))
            {
                throw new ArgumentException("Player is no member of this lobby.");
            }
            Logic.RemovePlayerFromLobby(new[] { player }, lobby).Wait();

            return "Player '" + player.Name + "' removed from lobby '" + lobby.Name + "'!"; ;
        }

        private string Command_CloseLobby(int lobbyId)
        {
            var lobby = GetLobby(lobbyId);

            Logic.CloseLobby(lobby).Wait();

            return "Lobby closed!";
        }

        private string Command_LeaveLobby(int conId)
        {
            var con = GetLobbyConnection(conId);

            Logic.LeaveLobby(con).Wait();

            return "Left lobby!";
        }

        private Lobby GetLobby(int id)
        {
            if(!Data.Lobbies.ContainsKey(id))
            {
                throw new ArgumentException("No lobby with this id found!");
            }
            return Data.Lobbies[id];
        }

        private IConnection GetConnection(int id)
        {
            if (!Server.ConnectionStorage.ConnectionExists(id))
            {
                throw new ArgumentException("No connection with this id found!");
            }
            var con = Server.ConnectionStorage.GetConnectionById(id);
            if (!Data.IsSynchronized(con))
            {
                throw new ArgumentException("Connection is not synchronized with this module!");
            }
            return con;
        }
        private LobbyConnection GetLobbyConnection(int id)
        {
            return Data.Connections[id];
        }

        private string Command_CreateLobby(int hostId, int gameId, string name)
        {
            var host = GetConnection(hostId);
            var game = Logic.GameDic[gameId];
            Logic.CreateLobby(game, Data.Connections[host.Id], name).Wait();
            return "Lobby created!";
        }
    }
}
