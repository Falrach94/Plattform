using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Control
{
    public interface IServerControl
    {
        public Task ResetNetworkAsync(int port);
        public Task ResetAllAsync(int port);


    }
}
