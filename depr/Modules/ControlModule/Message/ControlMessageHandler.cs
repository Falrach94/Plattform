using ControlModule;
using GameServer.Data_Objects;
using GameServerData;
using MessageUtilities;
using ServerImplementation.Control;
using ServerImplementation.ControlModule;
using System.Threading.Tasks;

namespace GameServer
{
    public class ControlMessageHandler : IModuleMessageHandler
    {

        public ControlMessageHandler(ControlData data, ControlLogic logic)
        {
            Logic = logic;
            Data = data;
        }

        public IMessenger Messenger { get; set; }
        public IServer Server { get; set; }

        public ControlLogic Logic { get; }
        public ControlData Data { get; }

        IModuleLogic IModuleMessageHandler.Logic => Logic;
        IModuleData IModuleMessageHandler.Data => Data;

        public string Name { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Task ProcessMessage(IConnection sender, Message msg)
        {
            return Task.CompletedTask;

        }

    }
}