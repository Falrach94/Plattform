using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement
{
    public interface IGameControl
    {
        Task GameOver(IGameResult result);
    }
}