using GameManagement.GameStorageModule.Data;
using GameSettingUtils;
using System.Collections.Generic;
using System.Linq;

namespace GameManagement.LobbyM.Data
{
    public class LobbyBackendWrapper
    {
        public GameWrapper Game { get; }
        public Dictionary<string, IGameModeData> GameModes { get; }
        public IGameModeData DefaultGameMode => GameModes.First().Value;

        public LobbyBackendWrapper(GameWrapper game)
        {
            Game = game;

            var provider = Game.Factory.CreateLobbySettingsProvider();

            var builder = new SettingsBuilder();

            provider.ProvideSettings(builder);

            GameModes = builder.CreateGameModeDic();
        }

    }
}