using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogModules.Interface
{
    public interface ILogModule
    {
        ILogManager LogManager { get; }
    }
}
