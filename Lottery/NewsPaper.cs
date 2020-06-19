using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    class NewsPaper
    {
        public void OnNewDrawPerformed(HashSet<int> lotteryNumbers)
        {
            string numbers = "";
            foreach (var number in lotteryNumbers)
            {
                numbers += number + " ";
            }
            Console.WriteLine("NewsPaper: resultados de la loteria --> {0}", numbers);
        }

        public void OnPrizeWon(string playerName, int hits)
        {
            Console.WriteLine("NewsPaper: {0} ha conseguido {1} aciertos", playerName, hits);
        }
    }
}
