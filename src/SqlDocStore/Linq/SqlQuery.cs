namespace SqlDocStore.Linq
{
    using System.Collections.Generic;

    public class SqlQuery
    {
        public SqlQuery()
        {
            Parameters = new Dictionary<string, object>();
        }

        public string Sql { get; set; }
        public IDictionary<string,object> Parameters { get; private set; }
    }
}
