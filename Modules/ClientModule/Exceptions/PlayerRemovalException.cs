using System;

namespace PlayerModule.Interface
{
    public class PlayerRemovalException : Exception
    {
        public PlayerRemovalException(string msg)
            : base(msg)
        {
        }
    }
}
