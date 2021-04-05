using ServerKernel.Data_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKernel.Connections.Manager
{
    public interface IConnectionProcessor
    {

        Task Synchronize(Connection connection);
        Task Disconnect(Connection connection);
    }
}
