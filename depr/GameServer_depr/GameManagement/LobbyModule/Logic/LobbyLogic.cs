using ChatModule;
using PlayerModule.Data;
using GameServer.Data_Objects;
using ServerImplementation.Framework.Module_Template;
using ServerImplementation.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerModule.Interface;
using System.Linq;
using System;
using GameServer;
using GameManagement.GameStorageModule.Data;
using GameManagement.LobbyM.Data;
using MessageUtilities;
using GameSettingUtils;

namespace GameManagement.LobbyM.Logic
{
    public class LobbyLogic : ModuleLogic<LobbyModuleData>, ILobbyLogic
    {
        public static LobbyMessageFactory Messages = LobbyMessageFactory.GetInstance();
        public IReadOnlyDictionary<int, GameWrapper> GameDic { get; set; }
        public IChatControl ChatControl { get; set; }
        public IPlayerStorage PlayerStorage {get; set; }
        public IGameManagement GameManagement { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            PlayerStorage.PlayerRemoved += PlayerStorage_PlayerRemoved;
        }


        public async Task SetGameSetting(Lobby lobby, IGameSetting setting, object value)
        {
            using (var l = await lobby.Mutex.Lock())
            {
                Data.SetSetting(setting, value);
                await UpdateLobbyConnections(lobby);
            }
        }

        internal Task ConfirmPendingUpdate(LobbyConnection lc)
        {
            if (Data.Connections[lc.Connection.Id].PendingUpdates == 0)
            {
                throw new ArgumentException();
            }

            Data.Connections[lc.Connection.Id].PendingUpdates--;

            return Task.CompletedTask;
        }


        private async Task UpdateLobbyConnections(Lobby lobby)
        {
            HashSet<Task> tasks = new HashSet<Task>();
            foreach(var c in lobby.Connections.Values)
            {
                tasks.Add(UpdateConnection(c));
            }
            await Task.WhenAll(tasks);
        }

        private async void PlayerStorage_PlayerRemoved(Player player)
        {
            var con = Data.Connections[player.Connection.Id];
            if(con.Lobby != null)
            {
                await RemovePlayerFromLobby(new[] { player }, con.Lobby);
            }
        }

        public override async Task SynchronizeConnection(IConnection connection)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Synchronizing))
            {
                await Messenger.SendMessage(connection, Messages.Lobbies(Data.Lobbies));
                Data.AddConnection(connection);
            }
        }
        public override async Task HandleDisconnect(IConnection connection)
        {
            log.Trace("handling disconnect (" + connection.Id + ")");
            var con = Data.Connections[connection.Id];

            if(con.Lobby != null)
            {
                await LeaveLobby(con, false);
            }

            Data.RemoveConnection(connection);
        }


        public async Task<Lobby> CreateLobby(GameWrapper game, LobbyConnection host, string name)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                log.Debug("creating new lobby '" + name + "' for host " + host);
                var lobby = new Lobby(name)
                {
                    Host = host,
                    Backend = new LobbyBackendWrapper(game)
                };

                await SetGameMode(lobby, lobby.Backend.DefaultGameMode);

                Data.AddLobby(lobby);

                try
                {
                    await Broadcast(Messages.LobbyCreated(lobby));

                    await JoinLobby(host, lobby);
                }
                catch
                {
                    Data.RemoveLobby(lobby);
                    throw;
                }

                return lobby;
            }
        }

        public async Task CloseLobby(Lobby lobby)
        {
            log.Debug("closing lobby " + lobby.ToDiagnosticShortString());
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                lobby.Host = null;//prevent host migration
                while(lobby.Connections.Count != 0)
                {
                    await LeaveLobby(lobby.Connections.Values.First(), true);
                }
                
                Data.RemoveLobby(lobby);
                await Broadcast(Messages.LobbyClosed(lobby));
            }
        }

        public Task StartGame(Lobby lobby)
        {
            if (lobby is null)
            {
                throw new ArgumentNullException(nameof(lobby));
            }
            if(lobby.Connections.Where(c => !c.Value.Ready).Any())
            {
                throw new ArgumentException("Not all players are ready!");
            }
            if(lobby.Players.Count < lobby.SelectedGameMode.MinPlayerNum)
            {
                throw new ArgumentException("Not enough players to start game!");
            }

            lobby.LobbyState = LobbyState.InGame;

            return GameManagement.StartGame(lobby);
        }

        public async Task SetConnectionReady(LobbyConnection connection, bool v)
        {
            if (connection.Lobby == null)
            {
                throw new ArgumentNullException();
            }
            Data.SetReady(connection, v);
            await LobbyBroadcast(connection.Lobby, Messages.ReadyChanged(connection));
        }

        public async Task MigrateHost(Lobby lobby)
        {
            log.Debug("migrating host");
            await CloseLobby(lobby);
        }
        public async override Task Reset()
        {
            log.Debug("resetting module");
            while(Data.Lobbies.Count != 0)
            {
                await CloseLobby(Data.Lobbies.First().Value);
            }
        }

        private Task LobbyBroadcast(Lobby lobby, Message message)
        {
            var tasks = new HashSet<Task>();
            foreach(var c in lobby.Connections.Values)
            {
                tasks.Add(Messenger.SendMessage(c.Connection, message));
            }
            return Task.WhenAll(tasks);
        }
        public async Task SetGameMode(Lobby lobby, IGameModeData mode)
        {
            log.Debug("setting gamemode of lobby " + lobby + " to " + mode.Name);
            using (var l = await lobby.Mutex.Lock())
            {
                /*lobby.GameSettings.Clear();
                foreach(var s in mode.Settings)
                {
                    lobby.GameSettings.Add(s.Key, s.Value);
                }*/
                lobby.SelectedGameMode = mode;
                await LobbyBroadcast(lobby, Messages.GameModeChanged(lobby));
                await UpdateLobbyConnections(lobby);
            }
        }

        public async Task JoinLobby(LobbyConnection c, Lobby l)
        {
            log.Debug("connection " + c + " joining lobby " + l);
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if(c.Lobby != null)
                {
                    throw new ArgumentException("Connection has already joined a lobby!");
                }
                if (l.LobbyState == LobbyState.Finished)
                {
                    throw new LobbyStateException("Can not join finished game!");
                }

                Data.AddLobbyConnection(l, c);

                try
                {
                    var players = PlayerStorage.GetPlayerOfConnection(c.Connection);
                    await AddPlayerToLobby(players, l);
                    await Messenger.SendMessage(c.Connection, Messages.LobbyJoined(l));

                    await Messenger.SendMessage(c.Connection, Messages.GameModeChanged(l));

                    await Messenger.SendMessage(c.Connection, Messages.ReadyStates(l));

                    //await Messenger.SendMessage(c.Connection, Messages.LobbySettings(l));

                    await UpdateConnection(c);

                }
                catch (Exception ex)
                {
                    log.Warn("connection " + c + " failed to join lobby " + l + ", reverting changes");
                    l.Connections.Remove(c.Connection.Id);
                    c.Lobby = null;
                    throw ex;
                }
            }
        }

        private async Task UpdateConnection(LobbyConnection lc)
        {
            lc.PendingUpdates++;
            await Messenger.SendMessage(lc.Connection, Messages.GameSettings(lc.Lobby));
            await SetConnectionReady(lc, false);
        }

        public async Task AddPlayerToLobby(IReadOnlyCollection<Player> players, Lobby lobby)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if(lobby.LobbyState != LobbyState.Open)
                {
                    throw new LobbyStateException("Can only add player to open lobbbies!");
                }
                if(lobby.OpenPlayerSlotNum < players.Count)
                {
                    throw new LobbyStateException("Not enough open player slots!");
                }
                foreach(var p in players)
                {
                    log.Debug("adding player " + p + " to lobby " + lobby);
                    if (!lobby.Connections.ContainsKey(p.Connection.Id))
                    {
                        throw new LobbyStateException("Connection " + p.Id + " is no member of this lobby!");
                    }
                    lobby.Players.Add(p);
                    await Broadcast(Messages.PlayerJoinedLobby(p, lobby));
                }
            }
        }

        public Task RemovePlayerFromLobby(IReadOnlyCollection<Player> players, Lobby lobby)
        {
            return RemovePlayerFromLobby(players, lobby, false);
        }
        public async Task RemovePlayerFromLobby(IReadOnlyCollection<Player> players, Lobby lobby, bool silent)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                foreach (var p in players)
                {
                    log.Debug("removing player " + p + " from lobby " + lobby);
                    if (!lobby.Players.Contains(p))
                    {
                        throw new LobbyStateException("Player " + p.Id + " is no member of this lobby!");
                    }
                    lobby.Players.Remove(p);
                    if (!silent)
                    {
                        await Broadcast(Messages.PlayerLeftLobby(p, lobby));
                    }
                }
            }
        }

        public Task LeaveLobby(LobbyConnection connection)
        {
            return LeaveLobby(connection, false);
        }
        public async Task LeaveLobby(LobbyConnection connection, bool silent)
        {
            log.Debug("removing connection " + connection + " from lobby " + connection.Lobby);

            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                var lobby = connection.Lobby;

                if(lobby == null)
                {
                    throw new LobbyStateException("Connection is not member in any lobby!");
                }

                log.Debug("connection " + connection.Connection.Id + " leaving lobby " + lobby.Id);

                if (PlayerStorage.IsSynchronized(connection.Connection))
                {
                    var players = PlayerStorage.GetPlayerOfConnection(connection.Connection).Where(p => lobby.Players.Contains(p)).ToArray();

                    await RemovePlayerFromLobby(players, lobby, silent);
                }

                lobby.Connections.Remove(connection.Connection.Id);

                if (!silent && connection.Connection.State != ConnectionState.Disconnected)
                {
                    await Messenger.SendMessage(connection.Connection, Messages.LobbyLeft());
                }
                connection.Lobby = null;
                connection.PendingUpdates = 0;

                if(lobby.Host == connection)
                {
                    await MigrateHost(lobby);
                }

            }
        }

    }
}