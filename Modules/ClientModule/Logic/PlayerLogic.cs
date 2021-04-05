using PlayerModule.Data;
using PlayerModule.Interface;
using GameServer.Data_Objects;
using GameServerData;
using ServerImplementation.Client;
using ServerImplementation.Framework.Module_Template;
using ServerImplementation.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SyncUtils.SynchronizingState;
using MessageUtilities;

namespace GameServer
{
    public class PlayerLogic : ModuleLogic<PlayerModuleData>
    {

        private static readonly PlayerMessageFactory Messages = PlayerMessageFactory.GetInstance();

        private async Task BroadcastMessage(Message msg)
        {
            HashSet<IConnection> connections = Messenger.Connections.Connections.Where((c) => Data.ConnectionDic.ContainsKey(c.Id)).ToHashSet();

            await Messenger.BroadcastMessage(connections, msg);
        }


        public async Task RenamePlayer(Player player, string newName)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                var old = player.Name;

                player.Name = newName;

                await BroadcastMessage(Messages.PlayerRenamed(player));

                log.Info("player '" + old + "' renamed " + player);
            }
        }

        public async Task<Player> AddPlayer(IConnection connection, string playerName)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                var newPlayer = Data.CreatePlayer(false, connection, playerName);

                Data.ConnectionDic[connection.Id].PlayerDic.Add(newPlayer.Id, newPlayer);

                await BroadcastMessage(Messages.PlayerAdded(newPlayer));

                log.Info("player added " + newPlayer);

                return newPlayer;
            }
        }
        public async Task RemovePlayer(IConnection connection, Player player)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                var playerInfo = Data.ConnectionDic[connection.Id];

                if (!playerInfo.PlayerDic.ContainsKey(player.Id))
                {
                    throw new PlayerRemovalException("Player " + player.Name + " does not belog to this connection!");
                }

                if (player.IsMain)
                {
                    log.Warn("failed to remove player " + player);
                    throw new PlayerRemovalException("Main player can't be removed!");
                }

                Data.RemovePlayer(player);
                playerInfo.PlayerDic.Remove(player.Id);

                await BroadcastMessage(Messages.PlayerRemoved(player));

                log.Info("player removed " + player);
            }
        }

        public override async Task HandleDisconnect(IConnection client)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if (Data.IsSynchronized(client))
                {
                    var players = Data.ConnectionDic[client.Id].Players;

                    Data.ConnectionDic.Remove(client.Id);

                    List<Task> tasks = new List<Task>();

                    foreach (var p in players)
                    {
                        Data.RemovePlayer(p);
                        tasks.Add(BroadcastMessage(Messages.PlayerRemoved(p)));
                    }

                    await Task.WhenAll(tasks);
                }
            }
        }

        public override async Task SynchronizeConnection(IConnection connection)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Synchronizing))
            {
                await Messenger.SendMessage(connection, Messages.Players(Data.Players));

                var mainPlayer = Data.CreatePlayer(true, connection);

                Data.ConnectionDic.Add(connection.Id, new ConnectionPlayerInfo(mainPlayer));

                await BroadcastMessage(Messages.PlayerAdded(mainPlayer));
            }
        }

        public override Task Reset()
        {
            return Task.CompletedTask;
        }
    }
}