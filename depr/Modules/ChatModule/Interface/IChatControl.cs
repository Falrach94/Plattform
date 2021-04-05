using ChatModule.Logic;
using GameServer.Data_Objects;
using System.Threading.Tasks;

namespace ChatModule
{
    public interface IChatControl
    {
        Task<ChatRoom> CreateChatRoom(string name, bool closeIfEmpty);
        Task CloseChatRoom(ChatRoom chat);
        Task SendMessage(ChatRoom chat, IConnection sender, string signature, string message);
        Task AddConnectionToChatRoom(ChatRoom chat, IConnection con);
        Task RemoveConnectionFromChat(ChatRoom chat, IConnection con, RemoveReason reason);
        //Task RenameChatRoom();
    }
}