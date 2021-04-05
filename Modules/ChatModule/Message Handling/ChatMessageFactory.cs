using GameServer.Data_Objects;
using GameServerData;
using GameServerData.ChatMessages;
using MessageUtilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChatModule.Message_Handling
{
    public class ChatMessageFactory : GameServer.Message_Handling.MessageFactory<ChatServerMessages>
    {
        private static ChatMessageFactory _instance;

        public override string Module => "Chat";

        private ChatMessageFactory() { }
        public static ChatMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ChatMessageFactory();
            }
            return _instance;
        }

        internal Message ChatClosed(ChatRoom chat)
        {
            var data = new SerializableStorage();
            data.Add("Id", chat.Id);
            return CreateMessage(ChatServerMessages.ChatRoomClosed, data);
        }

        internal Message NewMessage(ChatRoom chat, ChatMessage msg)
        {
            var data = new SerializableStorage();
            data.Add("ChatId", chat.Id);
            data.Add("Message", Tuple.Create(msg.Date, msg.Signature, msg.Text));
            return CreateMessage(ChatServerMessages.NewMessage, data);
        }

        internal Message NewChat(ChatRoom  chat)
        {
            var data = new SerializableStorage();
            
            data.Add("Chat", Tuple.Create(chat.Name, chat.Id, chat.Connections.Select(c=>c.Id).ToArray()));

            return CreateMessage(ChatServerMessages.ChatRoomOpened, data);
        }

        internal Message ConnectionLeftChat(ChatRoom chat, IConnection con)
        {
            var data = new SerializableStorage();

            data.Add("ChatId", chat.Id);
            data.Add("ConnectionId", con.Id);

            return CreateMessage(ChatServerMessages.ConnectionLeftChat, data);
        }

        internal Message OperationFailed(string message)
        {
            var data = new SerializableStorage();
            data.Add("Message", message);
            return CreateMessage(ChatServerMessages.Error, data);
        }

        internal Message ChatRoomDetails(ChatRoom chat)
        {
            var data = new SerializableStorage();

            data.Add("ChatId", chat.Id);
            data.Add("Name", chat.Name);
            data.Add("Connections", chat.Connections.Select(c => c.Id).ToArray());

            return CreateMessage(ChatServerMessages.ConnectionJoinedChat, data);
        }

        internal Message ConnectionJoinedChat(ChatRoom chat, IConnection con)
        {
            var data = new SerializableStorage();

            data.Add("ChatId", chat.Id);
            data.Add("ConnectionId", con.Id);

            return CreateMessage(ChatServerMessages.ConnectionJoinedChat, data);
        }

        internal Message RemovedFromChat(ChatRoom chat, string reason)
        {
            var data = new SerializableStorage();

            data.Add("ChatId", chat.Id);
            data.Add("Message", reason);

            return CreateMessage(ChatServerMessages.RemovedFromChat, data);
        }

        internal Message Chats(System.Collections.Generic.IEnumerable<ChatRoom> chats)
        {
            var data = new SerializableStorage();

            data.Add("Chats", chats.Select(chat => Tuple.Create(chat.Name, chat.Id, chat.Connections.Select(c=>c.Id).ToArray())).ToArray());
            return CreateMessage(ChatServerMessages.ChatRooms, data);
        }

        internal Message Success()
        {
            return CreateMessage(ChatServerMessages.Success, null);
        }

        internal Message ChatCreated(ChatRoom chat)
        {
            var data = new SerializableStorage();
            data.Add("Id", chat.Id);
            return CreateMessage(ChatServerMessages.Success, data);
        }
    }
}
