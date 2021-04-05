using GameServerData;
using MessageUtilities;
using System;

namespace GameServer.Message_Handling
{
    public abstract class MessageFactory<T> where T : Enum
    {
        public abstract string Module { get; }
        protected Message CreateMessage(T type, SerializableStorage data)
        {
            return new Message(Module, type.ToString(), data);
        }
    }
}
