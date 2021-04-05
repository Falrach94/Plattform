using GameServer.Data_Objects;
using GameServerData;
using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IMessenger
    {
        IConnectionStorage Connections { get; set; }
        Task<bool> SendMessage(IConnection receiver, Message msg);
        Task BroadcastMessage(Message msg);
        Task BroadcastMessage(IEnumerable<IConnection> receiver, Message msg);
        Task<bool> BroadcastMessageAndWait(IEnumerable<IConnection> receiver, Message msg, int timeoutMs);
        Task<Message> SendAndWaitForResponse(IConnection receiver, Message msg, int timeoutMs, string responseType = null);
        Task<bool> Respond(IConnection receiver, Message original, Message response);
        Task RespondWithError(IConnection receiver, Message original, int invalidConnectionState, string v);
        Task RespondWithError(IConnection receiver, Message message, Enum error, string desc);
        Task RespondWithSuccess(IConnection receiver, Message original, object result);
        Task RespondWithSuccess(IConnection receiver, Message original);
    }
}