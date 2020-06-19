using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            var tNorte = new Termometer("Norte");
            var tSur = new Termometer("Sur");


            var pComedor = new Screen("Comedor");
            tNorte.TemperatureChanged += pComedor.OnTemperatureChanged;
            tSur.TemperatureChanged += pComedor.OnTemperatureChanged;


            var pCocina = new Screen("Cocina");
            tNorte.TemperatureChanged += pCocina.OnTemperatureChanged;
            tSur.TemperatureChanged += pCocina.OnTemperatureChanged;


            tNorte.Set(22);
            tSur.Set(23);

            Console.ReadLine();
        }
    }
}
