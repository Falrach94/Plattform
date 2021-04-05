using ControlModule;
using GameServer;
using GameServer.Message_Handling;
using MessageUtilities;
using System.Collections.Generic;
using System.Linq;

namespace ServerImplementation.Control
{
    public class ControlMessageFactory : MessageFactory<ControlServerMessageType>
    {
        private static ControlMessageFactory _instance;

        public override string Module { get; } = "Control";

        private ControlMessageFactory() { }


        public static ControlMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ControlMessageFactory();
            }
            return _instance;
        }

        internal Message RightsLevel(ServerRightsLevel level)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("level", (int)level);
            return CreateMessage(ControlServerMessageType.AccessLevel, data);
        }

        internal Message UnsufficientPrivileges(ServerRightsLevel min)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("RequiredLevel", min);
            return CreateMessage(ControlServerMessageType.UnsufficientRights, data);
        }

        internal Message Modules(IReadOnlyCollection<IModule> modules)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("Modules", modules.Select((m) => m.Name).ToHashSet());
            return CreateMessage(ControlServerMessageType.Modules, data);
        }
    }
}
