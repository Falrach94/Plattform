using GameServer;
using System;

namespace ServerImplementation.Exceptions
{
    public class InvalidConnectionStateException : Exception
    {
        public ConnectionState State { get; }

        public InvalidConnectionStateException(ConnectionState state)
        {
            State = state;
        }
    }
}
