using GameManagement.LobbyM.Data;
using GameManagement.LobbyM.Messages;
using GameServer.Data_Objects;
using MessageUtilities;
using PlayerModule.Data;
using ServerImplementation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameManagement.LobbyM
{
    public class LobbyModuleMessageHandler : ModuleMessageHandler<Data.LobbyModuleData, Logic.LobbyLogic>
    {

        protected override void MessageRegistration()
        {
            RegisterMessage("CreateLobby",      Handle_CreateLobby,     typeof(Tuple<string, int>), "Lobby Name", "Game Id");
            RegisterMessage("JoinLobby",        Handle_JoinLobby,       typeof(int), "Lobby Id");
            RegisterMessage("LeaveLobby",       Handle_LeaveLobby,      null);
           
            RegisterMessage("ConfirmUpdate",    Handle_ConfirmUpdate,   null);
 
            RegisterMessage("SetGameSetting",   Handle_SetGameSetting,  typeof(Tuple<string,object>), "Setting Name", "Value");
            RegisterMessage("SetGameMode",      Handle_SetGameMode,     typeof(string), "Mode Name");

            RegisterMessage("Ready", Handle_Ready, null);

            RegisterMessage("StartGame", Handle_StartGame, null);
        }

        private Task Handle_StartGame(IConnection connection, Message message)
        {
            var lc = Data.Connections[connection.Id];

            if(lc.Lobby.Host != lc)
            {
                throw new ArgumentException("You must be host to start a game!");
            }

            return Logic.StartGame(lc.Lobby);
        }

        private Task Handle_Ready(IConnection connection, Message message)
        {
            var lc = Data.Connections[connection.Id];
            return Logic.SetConnectionReady(lc, !lc.Ready);
        }

        private void AssertHost(LobbyConnection lc)
        {
            if(lc.Lobby.Host != lc)
            {
                throw new ArgumentException("Connection is not lobby host!");
            }
        }

        private async Task Handle_SetGameMode(IConnection connection, Message message)
        {
            var lc = Data.Connections[connection.Id];

            AssertHost(lc);

            var name = message.Data.Get<string>("Data");

            var mode = lc.Lobby.Backend.GameModes[name];

            await Logic.SetGameMode(lc.Lobby, mode);
        }

        private async Task Handle_SetGameSetting(IConnection connection, Message message)
        {
            var lc = Data.Connections[connection.Id];

            AssertHost(lc);

            var s = message.Data.Get<Tuple<string, object>>("Data");

            var setting = lc.Lobby.GameSettings[s.Item1];

            await Logic.SetGameSetting(lc.Lobby, setting, s.Item2);
        }

        private Task Handle_ConfirmUpdate(IConnection connection, Message message)
        {
            return Logic.ConfirmPendingUpdate(Data.Connections[connection.Id]);
        }

        private async Task Handle_LeaveLobby(IConnection connection, Message message)
        {
            await Logic.LeaveLobby(Data.Connections[connection.Id]);
        }

        private async Task Handle_JoinLobby(IConnection connection, Message message)
        {
            int lobbyId = message.Data.Get<int>("Data");

            if (Data.Lobbies.ContainsKey(lobbyId))
            {
                await Logic.JoinLobby(Data.Connections[connection.Id], Data.Lobbies[lobbyId]);
            }
            else
            {
                await Messenger.RespondWithError(connection, message, Messages.LobbyError.LobbyIdNotFound, "No lobby with id " + lobbyId + " found!");                
            }
            await Messenger.RespondWithSuccess(connection, message);
        }

        private async Task Handle_CreateLobby(IConnection connection, Message message)
        {
            var con = Data.Connections[connection.Id];

            if (con.Lobby != null)
            {
                await Messenger.RespondWithError(connection, message, (int)Messages.LobbyError.InvalidConnectionState, "You can't create a new lobby while you're already joined in another.");
            }

            string name;
            int gameId; 
            (name, gameId) = message.Data.Get<Tuple<string,int>>("Data");

            if (!Logic.GameDic.ContainsKey(gameId))
            {
                await Messenger.RespondWithError(connection, message, (int)LobbyError.GameIdNotFound, "No game with this id found!");
            }
            var game = Logic.GameDic[gameId];

            var lobby = await Logic.CreateLobby(game, con, name);

            await Messenger.RespondWithSuccess(connection, message, lobby.Id);

            await Messenger.RespondWithSuccess(connection, message);
        }

    }
}