using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public delegate void NewDrawPerformed(HashSet<int> lotteryNumbers); 
    public class LotteryGenerator
    {
        private static Random random;
        public event NewDrawPerformed NewDrawPerformed;

        public LotteryGenerator()
        {
            random = new Random();
        }

        public void NewDraw()
        {
            HashSet<int> numbers = GenerateNumbers();
            if (NewDrawPerformed != null)
                NewDrawPerformed(numbers);
        }

        public static HashSet<int> GenerateNumbers()
        {
            var numbers = new HashSet<int>();

            while (numbers.Count() < 6)
                numbers.Add(random.Next(1, 50));

            return numbers;
        }
    }
}
