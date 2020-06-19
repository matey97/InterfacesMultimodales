using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoObserver
{
    public class Screen
    {
        string deviceName;
        string text = "";

        public Screen(string name)
        {
            deviceName = name;
        }

        public void OnTemperatureChanged(string termName, double newTemp)
        {
            text = newTemp.ToString();
            Console.WriteLine("Screen {0}: recibida nueva temperatura de {1} --> {2}!", deviceName, termName, text);
        }
    }
}
