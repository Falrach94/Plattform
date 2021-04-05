using GameServer;
using GameServer.Data_Objects;
using GameServerData;
using MessageUtilities;
using System.Threading.Tasks;

namespace TextInterfaceModule
{
    public class TextInterfaceMessageHandler : IModuleMessageHandler
    {
        public TextInterfaceMessageHandler(TextInterfaceData data, TextInterfaceLogic logic)
        {
            Logic = logic;
            Data = data;
        }

        public IMessenger Messenger { get; set; }

        public TextInterfaceLogic Logic { get; }
        public TextInterfaceData Data { get; }
        public IServer Server { get; set; }
        public string Name { get; set; } = "TextInterface";

        IModuleLogic IModuleMessageHandler.Logic => Logic;
        IModuleData IModuleMessageHandler.Data => Data;

        public Task ProcessMessage(IConnection sender, Message msg)
        {
            return Task.CompletedTask;
        }
    }
}