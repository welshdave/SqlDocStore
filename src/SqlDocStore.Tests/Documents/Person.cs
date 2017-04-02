using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDocStore.Tests.Documents
{
    public class Person
    {
        public Person()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string PreferredName { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}
