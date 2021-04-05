using ChatModule.Data;
using ChatModule.Logic;
using GameServer.Data_Objects;
using GameServerData;
using GameServerData.ChatMessages;
using MessageUtilities;
using ServerImplementation;
using System;
using System.Threading.Tasks;

namespace ChatModule.Message_Handling
{
    public class ChatModuleMessageHandler : ModuleMessageHandler<ChatModuleData, ChatModuleLogic>
    {
        private readonly ChatMessageFactory Messages = ChatMessageFactory.GetInstance();

        protected override void MessageRegistration()
        {
            RegisterMessage("CreateChat", Handle_CreateChat, typeof(string), "Name");
            RegisterMessage("JoinChat", Handle_JoinChat, typeof(int), "Chat Id");
            RegisterMessage("LeaveChat", Handle_LeaveChat, typeof(int), "Chat Id");
            RegisterMessage("RemoveChat", Handle_RemoveChat, typeof(int), "Chat Id");
            RegisterMessage("SendMessage", Handle_SendMessage, typeof(Tuple<int, string, string>), "Chat Id", "Signature", "Message");
        }

        private async Task Handle_SendMessage(IConnection connection, Message message)
        {
            int chatId = message.Data.Get<int>("Id");
            string name = message.Data.Get<string>("Name");
            string text = message.Data.Get<string>("Text");

            await Logic.SendMessage(Data.ChatRoomDic[chatId], connection, name, text);
            await Messenger.Respond(connection, message, Messages.Success());
        }

        private async Task Handle_RemoveChat(IConnection connection, Message message)
        {
            int chatId = message.Data.Get<int>("Id");

            var chat = Data.ChatRoomDic[chatId];

            await Logic.CloseChatRoom(chat);
            await Messenger.Respond(connection, message, Messages.Success());
        }

        private async Task Handle_LeaveChat(IConnection connection, Message message)
        {
            int chatId = message.Data.Get<int>("Id");

            var chat = Data.ChatRoomDic[chatId];

            await Logic.RemoveConnectionFromChat(chat, connection, RemoveReason.Request);
            await Messenger.Respond(connection, message, Messages.Success());
        }

        private async Task Handle_JoinChat(IConnection connection, Message message)
        {
            int chatId = message.Data.Get<int>("Id");

            var chat = Data.ChatRoomDic[chatId];

            await Logic.AddConnectionToChatRoom(chat, connection);

            await Messenger.Respond(connection, message, Messages.Success());
        }

        private async Task Handle_CreateChat(IConnection connection, Message message)
        {
            string name = message.Data.Get<string>("Name");

            var chat = await Logic.CreateChatRoom(name, true);

            await Messenger.Respond(connection, message, Messages.ChatCreated(chat));

        }

    }
}
