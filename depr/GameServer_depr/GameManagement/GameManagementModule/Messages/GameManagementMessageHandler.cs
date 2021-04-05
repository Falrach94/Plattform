using GameManagement;
using GameServer.Data_Objects;
using MessageUtilities;
using ServerImplementation;
using System;
using System.Threading.Tasks;

namespace GameManagementModule
{
    public class GameManagementMessageHandler : ModuleMessageHandler<GameManagementData, GameManagementLogic>
    {
        protected override void MessageRegistration()
        {
            RegisterMessage("Custom", Handle_Custom, typeof(object), "Data");
        }

        private Task Handle_Custom(IConnection connection, Message message)
        {
            return Data.Connections[connection.Id].Game.Backend.ProccessMessage(message.Data.Get<object>("Data"));
        }
    }
}