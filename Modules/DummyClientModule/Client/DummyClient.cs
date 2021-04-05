using DiagnosticsModule.Interface;
using ServerImplementation.Utils;

namespace DummyClientModule.Client
{
    public class DummyClient : MinimalClient.MinimalClient, IDiagnosticDataObject
    {
        private readonly static IndexPool IndexPool = new IndexPool();
        private readonly IndexPool.PoolIndex _index = IndexPool.GetNext();
        public int Id => _index.Value;

        public string Name { get; set; }

        public DummyClient()
        {
            RegisterMessageHandler(new ChatMessageHandler());
            RegisterMessageHandler(new PlayerMessageHandler());
            RegisterMessageHandler(new LobbyMessageHandler());

            Name = "Dummy " + Id;
        }

        public override string ToString()
        {
            return "(" + Name + ", " + Id + ", " + ConnectionState + ")";
        }

        public string ToDiagnosticLongString()
        {
            return "(" + Name + ", " + Id + ", " + ConnectionState + ", log name: " + LogManager.InstanceName + ")";
        }

        public string ToDiagnosticShortString()
        {
            throw new System.NotImplementedException();
        }
    }
}
