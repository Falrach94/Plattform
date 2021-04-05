using ChatModule;
using ChatModule.Data;
using ChatModule.Logic;
using ChatModule.Message_Handling;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using ServerImplementation.Modules;
using System;
using System.Linq;
using System.Threading.Tasks;
using TextInterfaceModule.Interface;
using TextParser.Parser;

namespace ChatModuleM
{
    public class ChatModule : Module<ChatModuleLogic, ChatModuleData, ChatModuleMessageHandler>, IChatModule, IRegisterTextInterface, IDiagnosticsControl
    {
        public ChatModule() : base("Chat", "Chat")
        {
        }

        public IChatControl ChatControl => Logic;
        public IChatStorage ChatStorage => Data;

        public IDiagnosticsDataInterface Diagnostics => Data;

        public void RegisterTextInterface(ITextInterfaceControl textInterface)
        {
            textInterface.AddToken("Chats", "^chats");
            textInterface.AddToken("Chat", "^chat");
            textInterface.AddToken("Create", "^create");
            textInterface.AddToken("Remove", "^remove");
            textInterface.AddToken("To", "^to");
            textInterface.AddToken("From", "^from");
            textInterface.AddToken("Add", "^add");
            textInterface.AddToken("Remove", "^remove");
            textInterface.AddToken("Send", "^send");
            textInterface.AddToken("As", "^as");
            textInterface.AddToken("Details", "^details");
            textInterface.AddToken("History", "^history");

            textInterface.AddRule(ParserSequence.Start().Token("Chat").Token("History").String("ChatName").End(),
                (c) =>
                {
                    var chat = GetChat(c.Get<string>("ChatName"));
                    return ChatHistory(chat, -1);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Chat").Token("History").Number("ChatId").End(),
                (c) =>
                {
                    var chat = GetChat(c.Get<int>("ChatId"));
                    return ChatHistory(chat, -1);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Chat").Token("History").String("ChatName").Number("Max").End(),
                (c) =>
                {
                    var chat = GetChat(c.Get<string>("ChatName"));
                    return ChatHistory(chat, c.Get<int>("Max"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Chat").Token("History").Number("ChatId").Number("Max").End(),
                (c) =>
                {
                    var chat = GetChat(c.Get<int>("ChatId"));
                    return ChatHistory(chat, c.Get<int>("Max"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Chat").Token("Details").String("ChatName").End(),
                (c) =>
                {
                    ChatRoom chat = GetChat(c.Get<string>("ChatName"));

                    return ChatDetails(chat);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Chat").Token("Details").Number("ChatId").End(),
                (c) =>
                 {
                     ChatRoom chat = GetChat(c.Get<int>("ChatId"));

                     return ChatDetails(chat);
                 });
            textInterface.AddRule(ParserSequence.Start().Token("Send").String("Message").Token("To").String("ChatName").End(),
                (c) =>
                {
                    try
                    {
                        ChatRoom chat = GetChat(c.Get<string>("ChatName"));
                        return SendMessage(chat, c.Get<string>("Message"), null);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
            textInterface.AddRule(ParserSequence.Start().Token("Send").String("Message").Token("To").Number("ChatId").End(),
                (c) =>
                {
                    try
                    {
                        ChatRoom chat = GetChat(c.Get<int>("ChatId"));
                        return SendMessage(chat, c.Get<string>("Message"), null);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
            textInterface.AddRule(ParserSequence.Start().Token("Send").String("Message").Token("To").String("ChatName").Token("As").String("Signature").End(),
                (c) =>
                {
                    try
                    {
                        ChatRoom chat = GetChat(c.Get<string>("ChatName"));
                        return SendMessage(chat, c.Get<string>("Message"), c.Get<string>("Signature"));
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
            textInterface.AddRule(ParserSequence.Start().Token("Send").String("Message").Token("To").Number("ChatId").Token("As").String("Signature").End(),
                (c) =>
                {
                    try
                    {
                        ChatRoom chat = GetChat(c.Get<int>("ChatId"));
                        return SendMessage(chat, c.Get<string>("Message"), c.Get<string>("Signature"));
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
            textInterface.AddRule(ParserSequence.Start().Token("Chats").End(),
                (c) =>
                {
                    return Chats();
                });
            textInterface.AddRule(ParserSequence.Start().Token("Remove").Token("Chat").Number("Id").End(),
                (c) =>
                {
                    try
                    {
                        return CloseChat(GetChat(c.Get<int>("Id")));
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
            textInterface.AddRule(ParserSequence.Start().Token("Remove").Token("Chat").String("ChatName").End(),
                (c) =>
                {
                    try
                    {
                        return CloseChat(GetChat(c.Get<string>("ChatName")));
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Chat").String("Name").End(),
                (c) =>
                {
                    return CreateChat(c.Get<string>("Name"), false);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Create").Token("Chat").String("Name").Boolean("AutoClose").End(),
                (c) =>
                {
                    return CreateChat(c.Get<string>("Name"), c.Get<bool>("AutoClose"));
                });
            textInterface.AddRule(ParserSequence.Start().Token("Add").Number("ConId").Token("To").Token("Chat").Number("ChatId").End(),
                (c) =>
                {
                    var chat = GetChat(c.Get<int>("ChatId"));
                    return AddConnectionToChat(c.Get<int>("ConId"), chat);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Add").Number("ConId").Token("To").Token("Chat").String("ChatName").End(),
                (c) =>
                {
                    var chat = GetChat(c.Get<string>("ChatName"));
                    return AddConnectionToChat(c.Get<int>("ConId"), chat);
                });
            textInterface.AddRule(ParserSequence.Start().Token("Remove").Number("ConId").Token("From").Token("Chat").Number("ChatId").End(),
                (c) => RemoveConnectionFromChat(c.Get<int>("ConId"), GetChat(c.Get<int>("ChatId"))));
            textInterface.AddRule(ParserSequence.Start().Token("Remove").Number("ConId").Token("From").Token("Chat").String("ChatName").End(),
                (c) => RemoveConnectionFromChat(c.Get<int>("ConId"), GetChat(c.Get<int>("ChatId"))));



        }

        private string ChatHistory(ChatRoom chat, int max)
        {
            int skip = chat.ChatHistory.Count - max;
            if (skip < 0 || max == -1) skip = 0;

            string history = String.Join("\n", chat.ChatHistory.Skip(skip).Select(h => "   " + h.Date.ToShortTimeString() + " " + h.Signature + ": " + h.Text));

            return "chat history:\n" + history;
        }

        private string ChatDetails(ChatRoom chat)
        {
            var msg = "Chat '" + chat.Name + "' (Id: " + chat.Id + ", auto remove: " + chat.CloseIfEmpty + ")\n";

            msg += "   Connections (" + chat.Connections.Count + ")";

            if (chat.Connections.Count != 0)
            {
                msg += ": " + String.Join(", ", chat.Connections.Select(c => c.Id));
            }

            return msg;
        }

        private string SendMessage(ChatRoom chat, string msg, string signature)
        {
            Logic.SendMessage(chat, null, signature ?? "Server", msg).Wait();
            return "Sent message to " + chat + ".";
        }

        private string RemoveConnectionFromChat(int connectionId, ChatRoom chat)
        {
            IConnection connection;
            if (!Server.ConnectionStorage.ConnectionExists(connectionId))
            {
                return "No connection with id " + connectionId + " found!";
            }
            connection = Server.ConnectionStorage.GetConnectionById(connectionId);

            if (!Data.IsSynchronized(connection))
            {
                return "Connection " + connection.Id + " is not synchronized with chat module!";
            }

            Logic.RemoveConnectionFromChat(chat, connection, RemoveReason.Kick).Wait();

            return "Connection " + connectionId + " was removed from chat " + chat + "!";
        }

        private string AddConnectionToChat(int connectionId, ChatRoom chat)
        {
            IConnection connection;
            if (!Server.ConnectionStorage.ConnectionExists(connectionId))
            {
                return "No connection with id " + connectionId + " found!";
            }
            connection = Server.ConnectionStorage.GetConnectionById(connectionId);

            if (!Data.IsSynchronized(connection))
            {
                return "Connection " + connection.Id + " is not synchronized with chat module!";
            }

            Logic.AddConnectionToChatRoom(chat, connection).Wait();

            return "Connection " + connectionId + " was added to chat " + chat + "!";
        }

        private ChatRoom GetChat(int id)
        {
            if (!Data.ChatRoomDic.TryGetValue(id, out ChatRoom chat))
            {
                throw new Exception("No chat with id " + id + " found!");
            }
            return chat;
        }
        private ChatRoom GetChat(string name)
        {
            ChatRoom chat = Data.GetChatWithName(name);
            if (chat == null)
            {
                throw new Exception("No chat with name '" + name + "' found!");
            }
            return chat;
        }
        private string Chats()
        {
            string output = "Chats (" + Data.ChatRoomDic.Count + ")";

            if (Data.ChatRoomDic.Count != 0)
            {
                output += "\n   ";
            }

            output += String.Join("\n   ", Data.ChatRoomDic.Values);

            return output;
        }

        private string CloseChat(ChatRoom chat)
        {
            Logic.CloseChatRoom(chat).Wait();
            return "Chat room " + chat + " closed!";
        }

        private string CreateChat(string name, bool autoClose)
        {
            var task = Logic.CreateChatRoom(name, autoClose);
            task.Wait();
            var chat = task.Result;

            return "Chat room '" + chat.Name + "' created!";
        }
    }
}
