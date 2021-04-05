using DiagnosticsModule.Interface;
using ServerImplementation.ConnectionHandler;
using ServerImplementation.Modules;

namespace ServerImplementation.ConnectionModule
{
    public class ConnectionModule : Module<ConnectionLogic, ConnectionData, DefaultMessageHandler<ConnectionData, ConnectionLogic>>, IDiagnosticsControl
    {
        public ConnectionModule() : base("Connection Module", "Connection")
        {
        }

        public IDiagnosticsDataInterface Diagnostics => Data;
    }
}
