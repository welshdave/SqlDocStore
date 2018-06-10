namespace SqlDocStore.MsSql.Linq
{
    using System.Collections.Generic;
    using System.Text;

    internal class MsSqlQueryParts
    {
        public MsSqlQueryParts()
        {
            OrderByAsc = true;
            OrderBy = new List<string>();
            Fields = new List<string> {"Document", "Etag"};
        }

        public string From { get; set; }
        public string Where { get; set; }
        public List<string> OrderBy { get; set; }
        public bool OrderByAsc { get; set; }
        public List<string> Fields { get; set; }

        public static implicit operator string(MsSqlQueryParts sql)
        {
            return sql.ToString();
        }

        public override string ToString()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT {0} FROM {1}",
                string.Join(",", Fields),
                From);
            if (!string.IsNullOrEmpty(Where))
                sql.AppendFormat(" WHERE {0}", Where);
            if (OrderBy.Count > 0)
            {
                var asc = OrderByAsc ? "ASC" : "DESC";
                sql.AppendFormat(" ORDER BY {0} {1}",
                    string.Join(",", OrderBy),
                    asc);
            }

            sql.Append(";");

            return sql.ToString();
        }
    }
}