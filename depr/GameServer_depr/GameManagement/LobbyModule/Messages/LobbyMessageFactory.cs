using GameManagement.LobbyM.Data;
using MessageUtilities;
using MessageUtility.MessageDictionary;
using PlayerModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameManagement
{
    public class LobbyMessageFactory
    {
        private static LobbyMessageFactory _instance;

        private static readonly MessageDictionary Messages = MessageDictionary.GetInstance();

        internal static LobbyMessageFactory GetInstance()
        {
            if(_instance == null)
            {
                _instance = new LobbyMessageFactory();
            }
            return _instance;
        }
        private LobbyMessageFactory() 
        {
            Messages.AddOutgoingMessage("Lobby", "Lobbies", (obj) =>
            {
                var lobbies = obj[0] as IReadOnlyDictionary<int, Lobby>;

                return lobbies.Values.Select(l =>
                {
                    var playerIds = l.Players.Select(p => p.Id).ToArray();
                    var hostId = l.Host.Connection.Id;
                    return Tuple.Create(l.Id, l.Name, l.Backend.Game.Id, hostId, playerIds);
                }
                ).ToArray();
            }, typeof(Tuple<int, string, int, int, int[]>[]), "Lobby Id", "Lobby Name", "Game Id", "Host Id", "Player Ids");

            Messages.AddOutgoingMessage("Lobby", "PlayerJoinedLobby", (obj) =>
            {
                var player = (Player)obj[0];
                var lobby = (Lobby)obj[1];
                return Tuple.Create(player.Id, lobby.Id);
            }, typeof(Tuple<int, int>), "Player Id", "Lobby Id");
            Messages.AddOutgoingMessage("Lobby", "YouJoinedLobby", (obj) =>
            {
                var lobby = (Lobby)obj[0];
                return lobby.Id;
            }, typeof(int), "Lobby Id");            
            Messages.AddOutgoingMessage("Lobby", "YouLeftLobby", null, null);

            Messages.AddOutgoingMessage("Lobby", "LobbyCreated", (obj) =>
            {
                var lobby = (Lobby)obj[0];
                var playerIds = lobby.Players.Select(p => p.Id).ToArray();
                var hostId = lobby.Host.Connection.Id;

                return Tuple.Create(lobby.Id, lobby.Name, lobby.Backend.Game.Id, hostId, playerIds);
            }, typeof(Tuple<int, int, int, int[]>), "Lobby Id", "Lobby Name", "Game Id", "Host Id", "Player Ids");

            Messages.AddOutgoingMessage("Lobby", "PlayerLeftLobby", (obj) =>
            {
                var player = (Player)obj[0];
                var lobby = (Lobby)obj[1];
                return Tuple.Create(player.Id, lobby.Id);
            }, typeof(Tuple<int, int>), "Player Id", "Lobby Id");

            Messages.AddOutgoingMessage("Lobby", "LobbyClosed", (obj) =>
            {
                var lobby = (Lobby)obj[0];
                return lobby.Id;
            }, typeof(int), "Lobby Id");

            Messages.AddOutgoingMessage("Lobby", "GameSettings", (obj) =>
            {
                var lobby = (Lobby)obj[0];
                return lobby.GameSettings.Select(p => Tuple.Create(p.Key, p.Value.Value)).ToArray();
            }, typeof(Tuple<string,object>[]), "Name", "Value");
            Messages.AddOutgoingMessage("Lobby", "GameModeChanged", (obj) =>
            {
                var lobby = (Lobby)obj[0];
                return lobby.SelectedGameMode.Name;
            }, typeof(string), "Gamemode Name");
            Messages.AddOutgoingMessage("Lobby", "SetReady", (obj) =>
            {
                var lc = (LobbyConnection)obj[0];
                return Tuple.Create(lc.Connection.Id, lc.Ready);
            }, typeof(Tuple<int, bool>), "Ready");
            Messages.AddOutgoingMessage("Lobby", "ReadyStates", (obj) =>
            {
                var lobby = (Lobby)obj[0];
                return lobby.Connections.Select(c => Tuple.Create(c.Key, c.Value.Ready)).ToArray();
            }, typeof(Tuple<int, bool>), "Connection ID", "Ready");

            /*Messages.AddOutgoingMessage("Lobby", "Lobby Settings", (obj) =>
            {
                var lobby = (Lobby)obj[0];
             //   var options = lobby.Logic.GameSettings.Select(p => Tuple.Create(p.Key, p.Value)).ToArray();
             //   var gameMode = lobby.Logic.GameMode.Name;
                return ;
            }, typeof(Tuple<Tuple<string, object>[], ), "Lobby Settings", "-Name", "-value", );
            */
        }

        internal Message ReadyChanged(LobbyConnection lc)
        {
            return Messages.CreateMessage("SetReady", lc);
        }

        internal Message LobbyLeft()
        {
            return Messages.CreateMessage("YouLeftLobby");
        }
        internal Message LobbyJoined(Lobby l)
        {
            return Messages.CreateMessage("YouJoinedLobby", l);
        }

        internal Message Lobbies(IReadOnlyDictionary<int, Lobby> lobbies)
        {
            return Messages.CreateMessage("Lobbies", lobbies);
        }
        internal Message PlayerJoinedLobby(Player player, Lobby lobby)
        {
            return Messages.CreateMessage("PlayerJoinedLobby", player, lobby);
        }

        internal Message LobbyCreated(Lobby lobby)
        {
            return Messages.CreateMessage("LobbyCreated", lobby);
        }

        internal Message PlayerLeftLobby(Player player, Lobby lobby)
        {
            return Messages.CreateMessage("PlayerLeftLobby", player, lobby);
        }

        internal Message LobbyClosed(Lobby lobby)
        {
            return Messages.CreateMessage("LobbyClosed", lobby);
        }

        internal Message LobbySettings(Lobby lobby)
        {
            return Messages.CreateMessage("LobbySettings", lobby);
        }

        internal Message GameSettings(Lobby l)
        {
            return Messages.CreateMessage("GameSettings", l);
        }
        internal Message GameModeChanged(Lobby l)
        {
            return Messages.CreateMessage("GameModeChanged", l);
        }

        internal Message ReadyStates(Lobby l)
        {
            return Messages.CreateMessage("ReadyStates", l);
        }
    }
}