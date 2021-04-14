
using NetworkUtils.Endpoint;
using SyncUtils;
using System;
using System.Collections.Generic;

namespace ServerKernel.Data_Objects
{
    public class Connection : IDisposable
    {
        public IDisposable IdReturner { get; set; }
        public int Id { get; set; }
        public ConnectionState State { get; set; } = ConnectionState.Connecting;
        public IEndpoint Endpoint { get; set; }

        public void Dispose()
        {
            if(IdReturner != null)
            {
                IdReturner.Dispose();
                IdReturner = null;

                Id = -1;
            }
        }

        public override string ToString()
        {
            return $"({Id}:{State})";
        }

    }
}
