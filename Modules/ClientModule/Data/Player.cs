using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using ServerImplementation.Utils;

namespace PlayerModule.Data
{
    public class Player : IDiagnosticDataObject
    {
        private static readonly IndexPool IndexPool = new IndexPool();
        private readonly IndexPool.PoolIndex _index = IndexPool.GetNext();
        public int Id => _index.Value;

        public bool IsMain { get; }

        public IConnection Connection { get; set; }
        public string Name { get; set; }

        public Player(bool main)
        {
            Name = "Player " + Id;
            IsMain = main;
        }

        public override string ToString()
        {
            return "(" + Name + ", " + Id + ")";
        }
        public string ToLongString()
        {
            return "(" + Name + ", id: " + Id + ", main: " + IsMain + ")";
        }

        public string ToDiagnosticLongString()
        {
            return "(" + Name + ", id: " + Id + ", main: " + IsMain + ", connection: " + Connection.Id + ")";
        }
        public string ToDiagnosticShortString()
        {
            return Id.ToString();
        }
    }
}
