using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public class NewsPaper
    {
        Dictionary<int, int> results = new Dictionary<int, int>();

        public void OnNewDraw(HashSet<int> numbers)
        {
            for (var aciertos = 3; aciertos <= 6; aciertos++)
                if (results.ContainsKey(aciertos))
                    Console.WriteLine("Acertantes de {0} números: {1}", aciertos, results[aciertos]);
                else
                    Console.WriteLine("Acertantes de {0} números: No hay ningún acertante", aciertos);
        }

        public void OnNewPrize(string name, int aciertos)
        {
            if (results.ContainsKey(aciertos))
                results[aciertos] += 1;
            else
                results[aciertos] = 1;
        }

    }
}
