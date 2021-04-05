using System;

namespace PlayerModule.Interface
{
    public class PlayerNotFoundException : Exception
    {
        public int Id { get; }
        public PlayerNotFoundException(int id)
            : base("No player with id " + id + " found!")
        {
            Id = id;
        }
    }
}
