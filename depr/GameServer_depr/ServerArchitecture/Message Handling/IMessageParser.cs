using GameServer.Data_Objects;
using MessageUtilities;
using System.IO;
using System.Threading.Tasks;

namespace GameServer
{
    public interface IMessageParser
    {
        IConnectionHandler ConnectionHandler { get; set; }
        IModuleControl Modules { get; set; }
        Message ParseMessage(Stream stream);
        Task HandleMessage(IConnection sender, Message msg);
    }
}