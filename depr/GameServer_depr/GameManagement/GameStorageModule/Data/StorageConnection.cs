using DiagnosticsModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.GameStorageModule.Data
{
    public class StorageConnection : IDiagnosticDataObject
    {
        public ISet<GameWrapper> SupportedGames { get; } = new HashSet<GameWrapper>();

        public string ToDiagnosticLongString()
        {
            return "";
        }

        public string ToDiagnosticShortString()
        {
            return "";
        }
    }
}
