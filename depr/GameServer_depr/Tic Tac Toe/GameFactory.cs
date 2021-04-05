using TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameUtilities.ServerBackend;

namespace GameServer
{
    public class GameFactory : ServerGameFactoryTemplate
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
