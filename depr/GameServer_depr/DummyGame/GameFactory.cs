using DummyGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class GameFactory : GameFactoryTemplate
    {
        public GameFactory() : base("Tic Tac Toe")
        {
        }

        protected override void RegisterGameModes()
        {
            AddGameMode<DefaultModeInfo>();
        }
    }
}
