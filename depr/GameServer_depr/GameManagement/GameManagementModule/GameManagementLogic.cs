using GameManagement.GameStorageModule.Data;
using GameManagement.LobbyM.Data;
using GameServer;
using GameServer.Data_Objects;
using LobbyModule.GameManagementModule.Messages;
using MessageUtilities;
using MessageUtility.MessageDictionary;
using PlayerModule.Data;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameManagement
{
    public class GameManagementLogic : ModuleLogic<GameManagementData>, IGameManagement
    {
        
        class GameControl : IGameControl
        {
            private GameInstance _game;
            private GameManagementLogic _logic;

            public GameControl(GameInstance game, GameManagementLogic logic)
            {
                _game = game;
                _logic = logic;
            }

            public Task GameOver(IGameResult result)
            {
                return _logic.EndGame(_game, result);
            }
        }

        class GameMessenger : IGameMessenger
        {
            private ICollection<LobbyConnection> _connections; 
            private IMessenger _messenger;
            public GameMessenger(ICollection<LobbyConnection> connections, IMessenger messenger)
            {
                _connections = connections;
                _messenger = messenger;
            }

            private Task Send(IConnection connection, object msg)
            {
                var data = new SerializableStorage();
                data.Add("Data", msg);
                return _messenger.SendMessage(connection, new Message("Game", "Custom", data));
            }

            public async Task Broadcast(object msg)
            {
                List<Task> tasks = new List<Task>();
                foreach(var c in _connections)
                {
                    tasks.Add(Send(c.Connection, msg));
                }

                await Task.WhenAll(tasks);
            }

            public Task Send(Player receiver, object msg)
            {
                return Send(receiver.Connection, msg);
            }
        }

        private static GameMessageFactory Messages { get; } = GameMessageFactory.GetInstance();


        public override Task Reset()
        {
            return Task.CompletedTask;
        }

        public override Task SynchronizeConnection(IConnection connection)
        {
            Data.AddGameConnection(connection);
            return Task.CompletedTask;
        }

        public override Task HandleDisconnect(IConnection connection)
        {
            Data.RemoveGameConnection(connection);
            return Task.CompletedTask;
        }

        private Task<bool> GameBroadcastAndWait(GameInstance game, Message msg)
        {
            return Messenger.BroadcastMessageAndWait(game.Connections.Select(c => c.Connection), msg, -1);

        }

        public async Task EndGame(GameInstance instance, IGameResult result)
        {
            instance.Result = result;
            Data.RemoveActiveGame(instance);

            await GameBroadcast(instance, Messages.GameOver());

            foreach(var c in instance.Connections)
            {
                c.Game = null;
            }

        }

        public async Task StartGame(Lobby lobby)
        {
            var game = lobby.Backend.Game;
            var instance = new GameInstance(game.Factory.CreateGameInstance(lobby.SelectedGameMode.Name),
                                            game.Name,
                                            lobby.Connections.Values.Select(c => Data.Connections[c.Connection.Id]).ToArray());

            Data.AddActiveGame(instance);

            foreach(var c in instance.Connections)
            {
                c.Game = instance;
            }

            var clientInitTask = GameBroadcastAndWait(instance, Messages.Initialize());

            await instance.Backend.Initialize(new GameControl(instance, this),
                                            new GameMessenger(lobby.Connections.Values, Messenger),
                                            lobby.SelectedGameMode,
                                            lobby.Players.ToArray());

            if (!await clientInitTask)
            {
                await AbortGame(instance);
            }
            else
            {
                await instance.Backend.Start();
                await GameBroadcast(instance, Messages.Start());
            }
        }

        private Task GameBroadcast(GameInstance game, Message msg)
        {
            return Messenger.BroadcastMessage(game.Connections.Select(c => c.Connection), msg);
        }

        public Task AbortGame(GameInstance game)
        {
            return game.Backend.Abort();
        }
    }
}