using PlayerModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement
{
    public interface IGameMessenger
    {
        Task Send(Player receiver, object msg);
        Task Broadcast(object msg);
    }
}