namespace SqlDocStore.Tests.Documents
{
    using System.Collections.Generic;

    public class Company
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Person> Officers { get; set; }
    }
}
