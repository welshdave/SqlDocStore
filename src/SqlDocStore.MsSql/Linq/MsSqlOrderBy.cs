namespace SqlDocStore.MsSql.Linq
{
    using System;
    using Remotion.Linq.Clauses;

    public class MsSqlOrderBy
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public OrderingDirection Direction { get; set; }
    }
}
