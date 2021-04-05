using DiagnosticsModule.Interface;
using GameServer.Data_Objects;
using ServerImplementation.Utils;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer
{
    internal class Connection : IConnection, IDiagnosticDataObject
    {
        private static readonly IndexPool _indexPool = new IndexPool();
        private readonly IndexPool.PoolIndex _poolIndex = _indexPool.GetNext();
        public int Id => _poolIndex.Value;

        public AsyncPulseSource StateMutex { get; } = new AsyncPulseSource();
        public ConnectionState State { get; set; } = ConnectionState.Connecting;


        public IDictionary<string, bool> ModuleDic { get; } = new Dictionary<string, bool>();


        public IEndpoint Endpoint { get; }

        public Connection(IEndpoint endpoint)
        {
            Endpoint = endpoint;
        }
        ~Connection()
        {
            _poolIndex.Dispose();
        }

        public override string ToString()
        {
            return "(Id: " + Id + ")";
        }

        public string ToDiagnosticLongString()
        {
            string output = "(Id: " + Id + ", " + State + ", ep connected: " + Endpoint.Connected + ")\n";

            output += "     modules: " + String.Join(", ", ModuleDic.Where(p => p.Value).Select(p => p.Key));

            return output;
        }

        public string ToDiagnosticShortString()
        {
            return Id.ToString();
        }
    }
}