
using System.Collections.Generic;

namespace GameServer.Data_Objects
{
    public interface IConnection
    {
        int Id { get; }
        IDictionary<string, bool> ModuleDic { get; }

        ConnectionState State { get; }
        AsyncPulseSource StateMutex { get; }

        IEndpoint Endpoint { get; }
    }
}
