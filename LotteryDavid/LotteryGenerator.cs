using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class LotteryGenerator
    {
        static readonly Random rnd = new Random();

        public event Action<HashSet<int>> NewDrawPerformed;

        public void NewDraw()
        {
            var numbers = new HashSet<int>();

            while (numbers.Count < 6)
                numbers.Add(rnd.Next(1, 50));

            Console.WriteLine(String.Join(",", numbers));

            if (NewDrawPerformed != null)
                NewDrawPerformed(numbers);
        }
    }
}
