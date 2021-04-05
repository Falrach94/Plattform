using PlayerModule.Interface;
using GameServer.Data_Objects;
using GameServerData;
using ServerImplementation;
using ServerImplementation.Client;
using System.Threading.Tasks;
using MessageUtilities;
using System;

namespace GameServer
{
    public class PlayerMessageHandler : ModuleMessageHandler<PlayerModuleData, PlayerLogic>
    {
        private static readonly PlayerMessageFactory Messages = PlayerMessageFactory.GetInstance();


        protected override void MessageRegistration()
        {
            RegisterMessage("AddPlayer", Handle_AddPlayer, typeof(string), "Name");
            RegisterMessage("RemovePlayer", Handle_RemovePlayer, typeof(int), "Player Id");
            RegisterMessage("RenamePlayer", Handle_RenamePlayer, typeof(Tuple<int, string>), "Player Id", "New Name");

        }
        private async Task Handle_RemovePlayer(IConnection sender, Message msg)
        {
            int id = msg.Data.Get<int>("Id");
            try
            {
                await Logic.RemovePlayer(sender, Data.GetPlayerById(id));
                await Messenger.Respond(sender, msg, Messages.Success());
            }
            catch (PlayerRemovalException ex)
            {
                await Messenger.Respond(sender, msg, Messages.RemovePlayerFailed(ex));
            }
        }

        private async Task Handle_RenamePlayer(IConnection sender, Message msg)
        {
            int id = msg.Data.Get<int>("Id");
            string name = msg.Data.Get<string>("Name");
            await Logic.RenamePlayer(Data.GetPlayerById(id), name);
            await Messenger.Respond(sender, msg, Messages.Success());
        }

        private async Task Handle_AddPlayer(IConnection sender, Message msg)
        {
            string name = msg.Data.Get<string>("Name");
            var player = await Logic.AddPlayer(sender, name);
            await Messenger.Respond(sender, msg, Messages.AddPlayerSuccess(player));
        }

    }
}