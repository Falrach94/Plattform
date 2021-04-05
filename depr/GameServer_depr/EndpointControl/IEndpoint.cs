using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    public interface IEndpoint
    {
        event EventHandler<byte[]> DataReceived;
        event EventHandler Disconnected;

        void Send(byte[] msg);
    }
}