using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SumaSerie
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(SumSeries(1, 10, Square));
            Console.WriteLine(SumSeries(1, 10, Cube));
            Console.ReadLine();
        }

        static double Square(double x)
        {
            return x * x;
        }

        static double Cube(double x)
        {
            return x * x * x;
        }

        delegate double DoubleFunction(double d); // equivalente a Func<double, double>
        // Func<double, void> no es válido --> Emplear Action<double>

        static double SumSeries(int from, int to, Func<double, double> f)
        {
            double sum = 0;
            for (int i = from; i <= to; i++)
                sum += f(i);
            return sum;
        }
    }
}
