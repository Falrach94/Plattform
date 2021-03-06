using MessageUtilities;
using ServerClientArchitecture;
using System.Threading.Tasks;

namespace DummyClientModule.Client
{
    public class LobbyMessageHandler : IMessageHandler
    {
        public string ModuleName => "Lobby";

        public IClient Client { get; set; }
        public Task ProcessMessage(Message msg)
        {
            return Task.CompletedTask;
        }

        public Task ShutDown()
        {
            return Task.CompletedTask;
        }
    }
}
