using GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyGame
{


    public class DefaultMode : SimpleMessageBackend
    {
        private Board _board;


        protected override Task<IGameResult> EndGame()
        {
            IGameResult result;
            int winner = _board.TestWinner();

            if(winner == -1)
            {
                result = new GameResult(true, null);
            }
            else
            {
                result = new GameResult(false, winner);
            }

            return Task.FromResult(result);
        }

        protected override Task InitGameMode(IReadOnlyDictionary<string, GameSetting> settings)
        {
            _board = new Board((int)settings["size"].Value);
            return Task.CompletedTask;
        }

        protected override void RegisterMessages()
        {
            RegisterMessage<Tuple<int, int>>("Set", Handle_Set);
        }

        private async Task Handle_Set(int p, Tuple<int, int> data)
        {
            try
            {
                _board.SetField(data.Item1, data.Item2, p);
                await Broadcast("Set", Tuple.Create(p, data.Item1, data.Item2));

                int winner = _board.TestWinner();
                if (winner != -1)
                {
                    await GameOver();
                }
            }
            catch
            {
                //TODO
            }
        }
    }
}
