using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    class Program
    {
        const int NUM_PLAYERS = 100000;

        static void Main(string[] args)
        {
            var lotteryGenerator = new LotteryGenerator();
            var newsPaper = new NewsPaper();

            var players = new List<Player>();

            for (var i=0; i<NUM_PLAYERS; ++i)
            {
                var player = new Player("Player_"+i.ToString());
                lotteryGenerator.NewDrawPerformed += player.OnNewDraw;
                player.PrizeWon += newsPaper.OnNewPrize;

                players.Add(player);
            }

            // Se debe enganchar el último
            lotteryGenerator.NewDrawPerformed += newsPaper.OnNewDraw;

            lotteryGenerator.NewDraw();
            Console.ReadLine();
        }
    }
}
