using System.Collections.Generic;

namespace ChatModule
{
    public interface IChatStorage
    {
        IReadOnlyDictionary<int, ChatRoom> ChatRoomDic { get; }
    }
}