using System;

namespace ServerImplementation.Exceptions
{
    public class WrongResponseTypeException : Exception
    {
        public string ExpectedType { get; }
        public string ReceivedType { get; }

        public WrongResponseTypeException(string expected, string received)
        {
            ExpectedType = expected;
            ReceivedType = received;
        }
    }
}
