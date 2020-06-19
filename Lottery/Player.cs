using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public delegate void PriceWon(string playerName, int hits);
    class Player
    {
        string name;
        HashSet<int> myNumbers;

        public event PriceWon PriceWon;
        public Player(string playerName)
        {
            name = playerName;
            myNumbers = LotteryGenerator.GenerateNumbers();
        }

        public void OnNewDrawPerformed(HashSet<int> lotteryNumbers)
        {
            var hits = lotteryNumbers.Intersect(myNumbers).Count();

            if (hits >= 5 && PriceWon != null)
                PriceWon(name, hits);
        }
    }
}
