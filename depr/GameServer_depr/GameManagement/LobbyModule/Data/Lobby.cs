using PlayerModule.Data;
using DiagnosticsModule;
using DiagnosticsModule.Interface;
using System.Collections.Generic;
using SyncUtils;
using ServerImplementation.Utils;
using GameSettingUtils;

namespace GameManagement.LobbyM.Data
{
    public class Lobby : IDiagnosticDataObject
    {
        private readonly static IndexPool IndexPool = new IndexPool();
        private readonly IndexPool.PoolIndex _index = IndexPool.GetNext();

        public Lobby(string name)
        {
            Name = name;
        }

        public AsyncMutex Mutex { get; } = new AsyncMutex();



        public List<Player> Players { get; } = new List<Player>();
        public Dictionary<int, LobbyConnection> Connections { get; } = new Dictionary<int, LobbyConnection>();

        #region lobby info
        public string Name { get; }
        public int Id => _index.Value;
        public int OpenPlayerSlotNum => SelectedGameMode.MaxPlayerNum - Players.Count;
        public LobbyConnection Host { get; set; }
        public LobbyState LobbyState { get; set; } = LobbyState.Open;
        #endregion

        #region Settings
        public LobbySettings LobbySettings { get; }
        public IGameModeData SelectedGameMode { get; set; }
        public IReadOnlyDictionary<string, IGameSetting> GameSettings => SelectedGameMode.Settings;//{ get; }  = new Dictionary<string, GameSetting>();
        #endregion

        public LobbyBackendWrapper Backend { get; set; }

        public string ToDiagnosticLongString()
        {
            string output = "(Id: " + Id + ", HostId: " + Host.Connection.Id + ", game: " + Backend.Game.Factory.Name + ")\n";
            output += DiagnosticsUtils.ListInternalString("member: ", Connections.Values, true);
            output += DiagnosticsUtils.ListInternalString("players: ", Players);
            return output;
        }

        public string ToDiagnosticShortString()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return Id.ToString();
        }

    }
}
