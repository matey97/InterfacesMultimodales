using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class Player
    {
        static readonly Random rnd = new Random();

        public event Action<string, int> PrizeWon;
        public string Name { get; private set; }

        private readonly HashSet<int> myNumbers;
        
        public Player(string name)
        {
            Name = name;
            myNumbers = new HashSet<int>();
            
            while (myNumbers.Count < 6)
                myNumbers.Add(rnd.Next(1, 50));
        }

        public void OnNewDraw(HashSet<int> numbers)
        {
            int aciertos = 0;
            
            foreach (var num in numbers)
                if (myNumbers.Contains(num))
                    aciertos += 1;

            if (aciertos >= 3 && PrizeWon != null)
                PrizeWon(Name, aciertos);
        }
    }
}
