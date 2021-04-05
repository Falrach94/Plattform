using DiagnosticsModule.Interface;
using GameServer.Data_Objects;

namespace GameManagement.LobbyM.Data
{
    public class LobbyConnection : IDiagnosticDataObject
    {
        public LobbyConnection(IConnection connection)
        {
            Connection = connection;
        }

        public int PendingUpdates { get; set; } = 1;
        public bool Synchronized => PendingUpdates == 0;

        public bool Ready { get; set; }

        public IConnection Connection { get; }
        public Lobby Lobby { get; set; }



        public string ToDiagnosticLongString()
        {
            return "(Id: " + Connection.Id + ", synchronized: " + Synchronized + ", lobby: " + (Lobby != null ? Lobby.Name : "none") + ")";
        }

        public string ToDiagnosticShortString()
        {
            return Connection.Id.ToString();
        }

        public override string ToString()
        {
            return Connection.Id.ToString();
        }
    }
}
