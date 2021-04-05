using System.Threading.Tasks;

namespace GameServer
{
    public interface IEndpointManager
    {
        IMessageParser MessageParser { get; set; }
        IConnectionHandler ConnectionHandler { get; set; }

        void StartListeningForConnections(int port);
        Task StopListeningForConnections();
    }
}