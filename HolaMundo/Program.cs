using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace HolaMundo
{
    class Program
    {
        static void Main(string[] args)
        {
            // cw
            Console.WriteLine("¡Hola mundo!");

            /*
            int a, b, result;
            string line;

            line = Console.ReadLine();
            a = int.Parse(line);

            line = Console.ReadLine();
            b = int.Parse(line);

            result = a + b;
            Console.WriteLine("La suma de {0} y {1} es {2}.", a, b, result);
            Console.WriteLine("Resulta {2} de sumar {0} y {1}.", a, b, result);
            */

            List<int> l1 = new List<int>();
            l1.Add(7);
            l1.Add(12);
            Console.WriteLine(l1.Count);
            Console.WriteLine(l1[0]);
            l1[0] = 123;
            Console.WriteLine(l1[0]);
            l1.Remove(123);
            l1.RemoveAt(0);


            Console.ReadLine();
        }
    }
}
