using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda
{
    public enum SexEnum { Man, Woman, Unknown };
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsFriend { get; set; }
        public SexEnum Sex { get; set; }

        public Person()
        {

        }

        public Person(string firstName, string lastName, bool isFriend, SexEnum sex)
        {
            FirstName = firstName;
            LastName = lastName;
            IsFriend = isFriend;
            Sex = sex;
        }
    }
}
