using System;

namespace ServerImplementation.Exceptions
{
    public class NotSynchronizedException : Exception
    {
        public NotSynchronizedException(string msg)
            : base(msg)
        {

        }
    }
}
