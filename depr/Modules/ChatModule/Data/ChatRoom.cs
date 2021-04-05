using DiagnosticsModule;
using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using ServerImplementation.Utils;
using System.Collections.Generic;

namespace ChatModule
{
    public class ChatRoom : IDiagnosticDataObject
    {
        private readonly static IndexPool IndexPool = new IndexPool();
        private readonly IndexPool.PoolIndex _index = IndexPool.GetNext();
        public int Id => _index.Value;

        public bool IsEmpty => Connections.Count == 0;

        public bool CloseIfEmpty { get; set; }

        public HashSet<IConnection> Connections { get; } = new HashSet<IConnection>();

        public List<ChatMessage> ChatHistory { get; } = new List<ChatMessage>();


        public string Name { get; set; }

        public string ToDiagnosticLongString()
        {
            string output = "(" + Name + ", " + Id + ", #msg " + ChatHistory.Count + ", auto close: " + CloseIfEmpty +")\n";
            output += "     "+DiagnosticsUtils.CollectionFlatString("connections", Connections, (c) => c.Id.ToString());
            return output;
        }

        public string ToDiagnosticShortString()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return "(" + Name + ", " + Id + ")";
        }
    }
}