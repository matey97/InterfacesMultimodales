using HumanResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birthdays
{
    class Program
    {
        static void Main(string[] args)
        {
            Person friend = new Person("María Jose", "Gil", new DateTime(1980, 10, 12));
            Employee somebody = new Employee("Pep", "Paz",new DateTime(1980, 10, 12),
                                              Employee.Level.Senior, "CEO");

            Console.WriteLine(somebody.ToString());
            Console.WriteLine("Edad: {0} años.",somebody.Age);
            Console.WriteLine("Iniciales: {0}", somebody.Initials);
            
            Console.ReadLine();
        }
    }
}
