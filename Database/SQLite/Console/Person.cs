using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }

        public Guid BulkInsertSessionId { get; set; }

        public byte[] TimeStamp { get; set; }

        public Person()
        {
            
        }

        public Person(int personId, string firstName, int age)
        {
            this.PersonId = personId;
            this.FirstName = firstName;
            this.Age = age;
        }

        public Person(int personId, string firstName, int age,Guid sessionId)
        {
            this.PersonId = personId;
            this.FirstName = firstName;
            this.Age = age;
            this.BulkInsertSessionId = sessionId;
        }

        public Person ShallowCopy()
        {
            return this.MemberwiseClone() as Person;
        }
    }
}
