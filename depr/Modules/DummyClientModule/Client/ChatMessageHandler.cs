using MessageUtilities;
using ServerClientArchitecture;
using System.Threading.Tasks;

namespace DummyClientModule.Client
{
    public class ChatMessageHandler : IMessageHandler
    {
        public string ModuleName => "Chat";

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
