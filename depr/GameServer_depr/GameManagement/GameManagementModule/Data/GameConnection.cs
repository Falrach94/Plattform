using DiagnosticsModule.Interface;
using GameManagement;
using GameServer.Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyModule.GameManagementModule.Data
{
    public class GameConnection : IDiagnosticDataObject
    {
        public GameConnection(IConnection connection)
        {
            Connection = connection;
        }

        public IConnection Connection { get; }
        public GameInstance Game { get; internal set; }

        public string ToDiagnosticLongString()
        {
            return Connection.Id.ToString();
        }

        public string ToDiagnosticShortString()
        {
            return Connection.ToString();
        }
    }
}
