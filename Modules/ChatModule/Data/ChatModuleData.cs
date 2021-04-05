using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using ServerImplementation.Framework.Module_Template;
using ServerImplementation.Modules;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatModule.Data
{
    public class ChatModuleData : ModuleData, IChatStorage, IDiagnosticsDataInterface
    {
        private readonly Dictionary<int, HashSet<ChatRoom>> _connectionChatDic = new Dictionary<int, HashSet<ChatRoom>>();

        private readonly Dictionary<int, ChatRoom> _chatRoomDic = new Dictionary<int, ChatRoom>();

        public IReadOnlyDictionary<int, ChatRoom> ChatRoomDic => _chatRoomDic;
        public IReadOnlyDictionary<int, HashSet<ChatRoom>> ConnectionChatRoomDic => _connectionChatDic;

        internal bool ContainsChat(string name)
        {
            return GetChatWithName(name) != null;
        }

        public ChatRoom GetChatWithName(string name)
        {
            var chat = _chatRoomDic.Values.Where(c => c.Name.Equals(name));
            if (!chat.Any())
            {
                return null;
            }
            return chat.First();
        }

        public override bool IsSynchronized(IConnection c)
        {
            return _connectionChatDic.ContainsKey(c.Id);
        }

        public void AddConnectionToChat(ChatRoom chat, IConnection con)
        {
            chat.Connections.Add(con);
            _connectionChatDic[con.Id].Add(chat);

            log.Debug("added " + con + " to " + chat);
        }
        public void RemoveConnectionFromChat(ChatRoom chat, IConnection con)
        {
            chat.Connections.Remove(con);
            _connectionChatDic[con.Id].Remove(chat);

            log.Debug("removed " + con + " from " + chat);
        }

        public void AddChat(ChatRoom chat)
        {
            _chatRoomDic.Add(chat.Id, chat);

            log.Debug("added chat " + chat);
        }

        public void RemoveChat(ChatRoom chat)
        {
            if (chat.Connections.Count != 0)
            {
                throw new InvalidOperationException("Chat rooms with open connections can't be removed!");
            }

            _chatRoomDic.Remove(chat.Id);

            log.Debug("removed chat " + chat);
        }

        public void ChangeChatName(ChatRoom chat, string name)
        {
            chat.Name = name;

            log.Debug("changed chat name " + chat);
        }

        public void AddMessage(ChatRoom chat, ChatMessage msg)
        {
            chat.ChatHistory.Add(msg);

            log.Debug("added message '" + msg + "' to " + chat);
        }

        internal void AddConnection(IConnection connection)
        {
            if (_connectionChatDic.ContainsKey(connection.Id))
            {
                throw new Exception();
            }
            _connectionChatDic.Add(connection.Id, new HashSet<ChatRoom>());

            log.Debug("Added chat connection " + connection);
        }

        internal void RemoveConnection(IConnection connection)
        {
            if (_connectionChatDic[connection.Id].Count != 0)
            {
                throw new Exception();
            }
            _connectionChatDic.Remove(connection.Id);

            log.Debug("Removed chat connection " + connection);
        }

        public string GetDetailedState()
        {
            string output = "";

            output += DiagnosticsUtils.ListString("chatrooms", _chatRoomDic) +"\n";

            output += DiagnosticsUtils.CollectionListString("synchronized connections", _connectionChatDic,
                (c) => DiagnosticsUtils.FlatString("chatrooms", c));

            return output;
        }
    }
}
