namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class MsSqlQueryParts
    {
        public MsSqlQueryParts()
        {
            OrderBy = new List<MsSqlOrderBy>();
            Fields = new List<string> {"doc.Document", "doc.Etag"};
        }

        public string From { get; set; }
        public string SubQuery { get; set; }
        public string Where { get; set; }
        public List<MsSqlOrderBy> OrderBy { get; set; }
        public List<string> Fields { get; set; }

        private readonly Dictionary<Type, string> Types = new Dictionary<Type, string>
        {
            {typeof(bool), "BIT"},
            {typeof(bool?), "BIT"},
            {typeof(byte), "TINYINT"},
            {typeof(byte?), "TINYINT"},
            {typeof(string), "NVARCHAR"},
            {typeof(DateTime), "DATETIME2"},
            {typeof(DateTime?), "DATETIME2"},
            {typeof(short), "SMALLINT"},
            {typeof(short?), "SMALLINT"},
            {typeof(int), "INT"},
            {typeof(int?), "INT"},
            {typeof(long), "BIGINT"},
            {typeof(long?), "BIGINT"},
            {typeof(decimal), "DECIMAL"},
            {typeof(decimal?), "DECIMAL"},
            {typeof(double), "FLOAT"},
            {typeof(double?), "FLOAT"},
            {typeof(float), "REAL"},
            {typeof(float?), "REAL"},
            {typeof(TimeSpan), "TIME"},
            {typeof(Guid), "UNIQUEIDENTIFIER"},
            {typeof(Guid?), "UNIQUEIDENTIFIER"},
            {typeof(byte[]), "BINARY"},
            {typeof(byte?[]), "BINARY"},
            {typeof(char[]), "VARCHAR"},
            {typeof(char?[]), "VARCHAR"}
        };

        public static implicit operator string(MsSqlQueryParts sql)
        {
            return sql.ToString();
        }

        public override string ToString()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT {0} FROM {1} {2}",
                string.Join(", ", Fields),
                From,
                SubQuery);
            if (!string.IsNullOrEmpty(Where))
                sql.AppendFormat(" WHERE {0}", Where);
            if (OrderBy.Count > 0)
            {
                var jsonOrderBy = new List<string>();

                foreach (var orderBy in OrderBy)
                    jsonOrderBy.Add(
                        $"CAST(JSON_VALUE(doc.Document, '$.{orderBy.Name}') AS {Types[orderBy.Type]}) {orderBy.Direction.ToString().ToUpper()}");

                sql.AppendFormat(" ORDER BY {0}",
                    string.Join(", ", jsonOrderBy));
            }

            sql.Append(";");

            return sql.ToString();
        }
    }
}