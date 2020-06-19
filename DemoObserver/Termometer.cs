using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoObserver
{
    public delegate void TemperatureChanged(string deviceName, double newTemperature);

    public class Termometer
    {
        string deviceName;
        double temp;

        public event TemperatureChanged TemperatureChanged;
        //public Action<double> TemperatureChanged;

        public Termometer(string name)
        {
            deviceName = name;
            temp = 0;
            Console.WriteLine("Termometer {0}: inicializado!", deviceName);
        }

        public void Set(double newTemp)
        {
            temp = newTemp;

            if (TemperatureChanged != null)
                TemperatureChanged(deviceName, temp);
            //TemperatureChanged?.Invoke(_temp);
            Console.WriteLine("Termometer {0}: cambiado --> {1}!", deviceName, temp);
        }
    }
}
