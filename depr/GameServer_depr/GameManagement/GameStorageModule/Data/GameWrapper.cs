using GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.GameStorageModule.Data
{
    public class GameWrapper
    {
        private static int NextId = 0;

        public GameWrapper(IServerGameFactory game)
        {
            Factory = game;
        }

        public string Name => Factory.Name;
        public int Id { get; } = NextId++;
        public IServerGameFactory Factory { get; }
    }
}
