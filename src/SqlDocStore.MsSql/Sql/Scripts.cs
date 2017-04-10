namespace SqlDocStore.MsSql.Sql
{
    using System.Collections.Concurrent;
    using System.IO;

    internal class Scripts
    {
        private readonly ConcurrentDictionary<string, string> _scripts
            = new ConcurrentDictionary<string, string>();

        internal readonly string Schema;
        internal readonly string Table;

        internal Scripts(string schema, string table)
        {
            Schema = schema;
            Table = table;
        }

        internal string CreateNonDefaultSchema => GetScript(nameof(CreateNonDefaultSchema));
        internal string CreateTable => GetScript(nameof(CreateTable));
        internal string InsertDocument => GetScript(nameof(InsertDocument));
        internal string UpdateDocument => GetScript(nameof(UpdateDocument));
        internal string UpsertDocument => GetScript(nameof(UpsertDocument));
        internal string DeleteDocument => GetScript(nameof(DeleteDocument));
        internal string GetDocumentById => GetScript(nameof(GetDocumentById));
        internal string PessimisticDeleteDocument => GetScript(nameof(PessimisticDeleteDocument));
       
        private string GetScript(string name)
        {
            return _scripts.GetOrAdd(name,
                key =>
                {
                    using (var stream = typeof(Scripts)
                        .Assembly
                        .GetManifestResourceStream("SqlDocStore.MsSql.Sql." + key + ".sql"))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader
                                .ReadToEnd()
                                .Replace("{@schema_name}", Schema)
                                .Replace("{@table_name}", Table);
                        }
                    }
                });
        }
    }
}