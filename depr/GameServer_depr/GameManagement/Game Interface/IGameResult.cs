using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameManagement
{
    public interface IGameResult
    {
        bool Aborted { get; }
        object Details { get; }
    }
}