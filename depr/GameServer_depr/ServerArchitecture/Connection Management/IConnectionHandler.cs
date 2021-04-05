using GameServer.Data_Objects;
using GameServer.Network;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IConnectionHandler
    {
        IModuleControl Modules { get; set; }
        IMessenger Messenger { get; set; }
        IConnectionStorage Data { get; }
        Task ProcessEndpoint(IConnection connection, EndpointChangedEventArgs e);

        Task CloseConnection(IConnection client, DisconnectReason reason, string message);
        Task ShutDown();
    }
}