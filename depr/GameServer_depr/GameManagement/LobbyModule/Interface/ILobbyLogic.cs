using GameManagement.GameStorageModule.Data;
using GameManagement.LobbyM.Data;
using GameSettingUtils;
using PlayerModule.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameManagement.LobbyM
{
    public interface ILobbyLogic
    {
        Task<Lobby> CreateLobby(GameWrapper game, LobbyConnection host, string name);
        Task CloseLobby(Lobby lobby);

        Task JoinLobby(LobbyConnection connection, Lobby lobby);
        Task LeaveLobby(LobbyConnection connection);

        Task AddPlayerToLobby(IReadOnlyCollection<Player> players, Lobby lobby);
        Task RemovePlayerFromLobby(IReadOnlyCollection<Player> players, Lobby lobby);

        Task SetConnectionReady(LobbyConnection connection, bool value);
        Task MigrateHost(Lobby lobby);
        Task StartGame(Lobby lobby);
        Task SetGameMode(Lobby lobby, IGameModeData mode);
    }
}