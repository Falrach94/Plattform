using ChatModule.Data;
using ChatModule.Message_Handling;
using GameServer.Data_Objects;
using GameServerData;
using ServerImplementation.Exceptions;
using ServerImplementation.Framework.Module_Template;
using ServerImplementation.Modules;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ChatModule.Logic
{
    public class ChatModuleLogic : ModuleLogic<ChatModuleData>, IChatControl
    {
        public readonly static ChatMessageFactory Messages = ChatMessageFactory.GetInstance();

        public async Task<ChatRoom> CreateChatRoom(string name, bool closeIfEmpty)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if (Data.ContainsChat(name))
                {
                    throw new ArgumentException("Chat name already in use!");
                }

                ChatRoom room = new ChatRoom()
                {
                    Name = name,
                    CloseIfEmpty = closeIfEmpty
                };

                Data.AddChat(room);

                await Broadcast(Messages.NewChat(room));

                return room;
            }
        }

        public async Task CloseChatRoom(ChatRoom chat)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                while (chat.Connections.Count != 0)
                {
                    await RemoveConnectionFromChat(chat, chat.Connections.First(), RemoveReason.ChatClosed);
                }
                Data.RemoveChat(chat);
                await Broadcast(Messages.ChatClosed(chat));
            }
        }

        private async Task ChatBroadcast(ChatRoom chat, MessageUtilities.Message msg)
        {
            await Messenger.BroadcastMessage(chat.Connections, msg);
        }

        public async Task SendMessage(ChatRoom chat, IConnection sender, string signature, string message)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if (signature == null)
                {
                    throw new ArgumentNullException();
                }

                if (sender != null && !chat.Connections.Contains(sender))
                {
                    throw new ArgumentException("Connection " + sender.Id + " is no member of chat " + chat.Id + ".");
                }
                var msg = new ChatMessage(message, signature, DateTime.Now);
                Data.AddMessage(chat, msg);
                await ChatBroadcast(chat, Messages.NewMessage(chat, msg));
            }
        }

        public async Task AddConnectionToChatRoom(ChatRoom chat, IConnection connection)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if (!Data.IsSynchronized(connection))
                {
                    throw new NotSynchronizedException("Connection " + connection.Id + " is not synchronized with chat module yet!");
                }
                if (chat.Connections.Contains(connection))
                {
                    throw new ArgumentException("Connection " + connection + " has already joined chat room " + chat + "!");
                }

                Data.AddConnectionToChat(chat, connection);

                await Broadcast(Messages.ConnectionJoinedChat(chat, connection));
            }
        }

        public async Task RemoveConnectionFromChat(ChatRoom chat, IConnection con, RemoveReason reason)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if (!chat.Connections.Contains(con))
                {
                    throw new ArgumentException("Connection " + con.Id + " is no member of chat " + chat.Id + ".");
                }

                Data.RemoveConnectionFromChat(chat, con);

                if (reason != RemoveReason.ChatClosed)
                {
                    await Broadcast(Messages.ConnectionLeftChat(chat, con));
                }

                if (reason != RemoveReason.Request)
                {
                    await Messenger.SendMessage(con, Messages.RemovedFromChat(chat, reason.ToString()));
                }

                if (chat.CloseIfEmpty && chat.IsEmpty)
                {
                    await CloseChatRoom(chat);
                }
            }
        }

        public Task ModifyChatRoom()
        {
            throw new NotImplementedException();
        }

        public override async Task HandleDisconnect(IConnection connection)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Writing))
            {
                if (!Data.IsSynchronized(connection))
                {
                    throw new ArgumentException();
                }

                var chats = Data.ConnectionChatRoomDic[connection.Id];
                while (chats.Count != 0)
                {
                    Data.RemoveConnectionFromChat(chats.First(), connection);
                }
                Data.RemoveConnection(connection);
            }
        }
        public override async Task SynchronizeConnection(IConnection connection)
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Synchronizing))
            {
                await Messenger.SendMessage(connection, Messages.Chats(Data.ChatRoomDic.Values));
                Data.AddConnection(connection);
            }
        }

        public override async Task Reset()
        {
            using (var alock = await Data.Synchronizer.SetStateAsync(ModuleState.Resetting))
            {
                while (Data.ChatRoomDic.Count != 0)
                {
                    await CloseChatRoom(Data.ChatRoomDic.Values.First());
                }
            }
        }
    }
}
