using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources
{
    public class Person
    {
        //prop
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public DateTime BirthDate { get; protected set; }
       
        //ctor
        public Person(string firstName, string lastName, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public int Age
        {
            get
            {
                DateTime today = DateTime.Now;
                int years = today.Year - BirthDate.Year - 1;
                if (today.Month > BirthDate.Month ||
                (today.Month == BirthDate.Month && today.Day >= BirthDate.Day))
                    years++;
                return years;
            }
        }

        public string Initials
        {
            get
            {
                string[] firstNameComponents = FirstName.Split(' ');
                string res = "";
                for (int i = 0; i < firstNameComponents.Length; i++)
                {
                    res += firstNameComponents[i][0] + ".";
                }
                return res + LastName[0] + ".";
            }
        }

        public override string ToString()
        {
            return FirstName + " " + LastName + " (" + BirthDate.Day + "/" +
                    BirthDate.Month + "/" + BirthDate.Year + ")";
        }
    }
}
