using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Network
{
    public interface INetwork
    {
        /// <summary>
        /// Module will try to start on this port
        /// </summary>
        public int TargetPort { get; set; }
        public int Port { get; }
    }
}
