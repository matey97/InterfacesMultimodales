using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    class Program
    {
        static void Main(string[] args)
        {
            LotteryGenerator lottery = new LotteryGenerator();
            NewsPaper newsPaper = new NewsPaper();

            lottery.NewDrawPerformed += newsPaper.OnNewDrawPerformed;

            Player player;
            for (var i = 0; i < 1000000; i++)
            {
                player = new Player("player" + i);
                lottery.NewDrawPerformed += player.OnNewDrawPerformed;
                player.PriceWon += newsPaper.OnPrizeWon;
            }

            lottery.NewDraw();

            Console.ReadLine();
        }
    }
}
