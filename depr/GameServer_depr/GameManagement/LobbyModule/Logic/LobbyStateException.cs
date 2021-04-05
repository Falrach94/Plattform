using System;
using System.Runtime.Serialization;

namespace GameManagement.LobbyM.Logic
{
    [Serializable]
    internal class LobbyStateException : Exception
    {
        public LobbyStateException()
        {
        }

        public LobbyStateException(string message) : base(message)
        {
        }

        public LobbyStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LobbyStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}