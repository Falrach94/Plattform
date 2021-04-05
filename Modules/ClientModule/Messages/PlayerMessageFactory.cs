using PlayerModule.Data;
using PlayerModule.Interface;
using GameServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using MessageUtilities;

namespace ServerImplementation.Client
{
    public class PlayerMessageFactory
    {
        private static PlayerMessageFactory _instance;

        private PlayerMessageFactory() { }

        public static PlayerMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PlayerMessageFactory();
            }
            return _instance;
        }
        private Message CreateMessage(PlayerServerMessageType type, SerializableStorage data)
        {
            return new Message("Player", type.ToString(), data);
        }


        internal Message Players(IReadOnlyCollection<Player> info)
        {
            var data = new SerializableStorage();

            var playerData = new HashSet<Tuple<string, int, int, bool>>();

            foreach (var p in info)
            {
                playerData.Add(Tuple.Create(p.Name, p.Connection.Id, p.Id, p.IsMain));
            }

            data.Add("Players", playerData.ToArray());

            return CreateMessage(PlayerServerMessageType.Players, data);
        }

        internal Message PlayerAdded(Player player)
        {
            var data = new SerializableStorage();

            data.Add("Player", Tuple.Create(player.Name, player.Connection.Id, player.Id, player.IsMain));

            return CreateMessage(PlayerServerMessageType.PlayerAdded, data);
        }

        internal Message PlayerRemoved(Player player)
        {
            var data = new SerializableStorage();

            data.Add("PlayerId", player.Id);

            return CreateMessage(PlayerServerMessageType.PlayerRemoved, data);
        }

        internal Message AddPlayerSuccess(Player player)
        {
            var data = new SerializableStorage();

            data.Add("Id", player.Id);

            return CreateMessage(PlayerServerMessageType.Success, data);
        }

        internal Message Success()
        {
            return CreateMessage(PlayerServerMessageType.Success, null);
        }

        internal Message PlayerRenamed(Player player)
        {
            var data = new SerializableStorage();

            data.Add("PlayerId", player.Id);
            data.Add("Name", player.Name);

            return CreateMessage(PlayerServerMessageType.PlayerRenamed, data);
        }

        internal Message RemovePlayerFailed(PlayerRemovalException ex)
        {
            var data = new SerializableStorage();

            data.Add("Message", ex.Message);

            return CreateMessage(PlayerServerMessageType.Error, data);
        }
    }
}
