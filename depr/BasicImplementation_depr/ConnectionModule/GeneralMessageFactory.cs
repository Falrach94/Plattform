using GameServer;
using GameServer.Data_Objects;
using GameServer.Message_Handling;
using GameServer.Network;
using GameServerData;
using MessageUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerImplementation.Client
{
    public class GeneralMessageFactory : MessageFactory<GeneralServerMessageType>
    {
        public override string Module { get; } = "General";
        private static GeneralMessageFactory _instance;


        private GeneralMessageFactory() { }

        public static GeneralMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GeneralMessageFactory();
            }
            return _instance;
        }


        public Message ConnectFinished(int id)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("Id", id);
            return CreateMessage(GeneralServerMessageType.ConnectFinished, data);
        }

        public Message ServerClosedConnection(string msg, DisconnectReason reason)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("Message", msg??"");
            data.Add("Reason", reason);
            return CreateMessage(GeneralServerMessageType.ServerClosedConnection, data);
        }

        public Message NewConnection(IConnection client)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("Id", client.Id);
            return CreateMessage(GeneralServerMessageType.NewConnection, data);
        }

        internal Message ClientDisconnected(Connection client)
        {
            SerializableStorage data = new SerializableStorage();
            data.Add("Id", client.Id);
            return CreateMessage(GeneralServerMessageType.ConnectionClosed, data);
        }

        internal Message AllConnections(IReadOnlyCollection<IConnection> set)
        {
            SerializableStorage data = new SerializableStorage();

            ISet<int> idSet = new HashSet<int>();
            idSet = set.Select((c) => c.Id).ToHashSet();

            data.Add("Connections", idSet);
            return CreateMessage(GeneralServerMessageType.AllConnections, data);
        }

        internal Message Modules(IReadOnlyCollection<IModule> loadedModules)
        {
            SerializableStorage data = new SerializableStorage();

            string[] modules = loadedModules.Select((m) => m.Name).ToArray();
            data.Add("Modules", modules);

            return CreateMessage(GeneralServerMessageType.ModulesRequest, data);
        }

        internal Message Hello(Connection connection)
        {
            SerializableStorage data = new SerializableStorage();

            data.Add("Data", connection.Id);

            return new Message(Module, "Hello", data);
        }
    }
}
