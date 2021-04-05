using DiagnosticsModule.Interface;
using GameManagement;
using LobbyModule.GameManagementModule.Data;
using ServerImplementation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement
{
    public class GameInstance : IDiagnosticDataObject
    {
        private static readonly IndexPool IndexPool = new IndexPool();
        private readonly IndexPool.PoolIndex _index = IndexPool.GetNext();
        public int Id => _index.Value;

        public GameInstance(IServerGameBackend model, string name, ICollection<GameConnection> connections)
        {
            Backend = model;
            Name = name;
            Connections = connections;
        }

        public ICollection<GameConnection> Connections { get; }

        public IServerGameBackend Backend { get; }
        public string Name { get; }
        public IGameResult Result { get; set; }


        public string ToDiagnosticLongString()
        {
            return "(Id: " + Id + ", '" + Name + "result set: " + (Result != null) + ")";
        }

        public string ToDiagnosticShortString()
        {
            return Id.ToString();
        }
    }
}
