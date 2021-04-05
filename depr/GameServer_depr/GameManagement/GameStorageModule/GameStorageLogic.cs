using GameManagement.Interface;
using GameServer.Data_Objects;
using GameManagement.GameStorageModule.Data;
using MessageUtility.MessageDictionary;
using ServerImplementation;
using ServerImplementation.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GameManagement.GameStorageModule
{
    public class GameStorageLogic : ModuleLogic<GameStorageData>, IGameModuleStorage
    {
        private static readonly MessageDictionary Messages = MessageDictionary.GetInstance();

        public IReadOnlyDictionary<int, GameWrapper> Games => Data.Games;

        public GameStorageLogic()
        {
            Messages.AddOutgoingMessage("Game Storage", "Games", (p) =>
            {
                var wrapper = (IEnumerable<GameWrapper>)p[0];
                return wrapper.Select(wr => Tuple.Create(wr.Id, wr.Factory.Name)).ToArray();
            },
            typeof(Tuple<int, string>[]), "Game Id", "Game Name");
            Messages.AddOutgoingMessage("Game Storage", "New Game", (p) =>
            {
                var wrapper = (GameWrapper)p[0];
                return Tuple.Create(wrapper.Id, wrapper.Factory.Name);
            },
            typeof(Tuple<int, string>), "Game Id", "Game Name");

            Messages.AddIngoingMessage("Game Storage", "Games", typeof(int[]), "IDs of supported games");
            Messages.AddIngoingMessage("Game Storage", "New Game", typeof(int), "ID of supported game");
        }

        public async override Task SynchronizeConnection(IConnection connection)
        {
            Data.AddConnection(connection);

            var response = await Messenger.SendAndWaitForResponse(connection, Messages.CreateMessage("Games", Data.Games.Values), Const.RESPONSE_TIMEOUT, "Games");

            var gameIds = response.Data.Get<int[]>("Data");
            var games = gameIds.Select(id => Games[id]).ToArray();
            Data.AddGameToConnection(connection, games);
        }
        public override Task HandleDisconnect(IConnection connection)
        {
            Data.RemoveConnection(connection);
            return Task.CompletedTask;
        }
        public void AddGame(IServerGameFactory game)
        {
            Data.AddModule(game);
        }

        public void LoadDll(string path)
        {
            var library = Assembly.LoadFrom(path);

            var game = (IServerGameFactory)Activator.CreateInstance(library.FullName, "GameServer.GameFactory").Unwrap();

            AddGame(game);
        }

        public override Task Reset()
        {
            return Task.CompletedTask;
        }
    }
}