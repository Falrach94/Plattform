using PlayerModule.Data;
using PlayerModule.Interface;
using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using ServerImplementation.Modules;
using System.Collections.Generic;
using System;

namespace GameServer
{
    public class PlayerModuleData : ModuleData, IPlayerStorage, IDiagnosticsDataInterface
    {
        public Dictionary<int, ConnectionPlayerInfo> ConnectionDic { get; set; } = new Dictionary<int, ConnectionPlayerInfo>();

        private readonly Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();

        public event Action<Player> PlayerRemoved;

        public IReadOnlyCollection<Player> Players => _playerDic.Values;

        public Player CreatePlayer(bool main, IConnection connection, string name = "")
        {
            var player = new Player(main)
            {
                Connection = connection,
                Name = name
            };
            if (name == "")
            {
                player.Name = "Player " + player.Id;
            }

            _playerDic.Add(player.Id, player);

            log.Debug("player added " + player);

            return player;

        }
        public void RemovePlayer(Player p)
        {
            PlayerRemoved?.Invoke(p);
            _playerDic.Remove(p.Id);
            log.Debug("player removed " + p);
        }

        public override bool IsSynchronized(IConnection connection)
        {
            return ConnectionDic.ContainsKey(connection.Id);
        }

        public ConnectionPlayerInfo GetPlayerInfo(IConnection connection)
        {
            return ConnectionDic[connection.Id];
        }

        public Player GetPlayerById(int id)
        {
            return _playerDic[id];
        }

        public bool PlayerExists(int id)
        {
            return _playerDic.ContainsKey(id);
        }

        public string GetDetailedState()
        {
            string output = string.Empty;

            output += DiagnosticsUtils.ListString("synchronized connections", ConnectionDic);
            /*
            output += DiagnosticsUtils.CollectionListString(
                "synchronized connections",
                ConnectionDic,
                c => "main: " + c.MainPlayer.Id + ", "
                + DiagnosticsUtils.CollectionFlatString("player", c.Players, p => p.Id.ToString()));
                */
            output += "\n";

            output += DiagnosticsUtils.CollectionListString("players ", _playerDic, p => p.ToDiagnosticLongString());

            return output;
        }

        public IReadOnlyCollection<Player> GetPlayerOfConnection(IConnection c)
        {
            return GetPlayerInfo(c).Players;
        }
    }
}
