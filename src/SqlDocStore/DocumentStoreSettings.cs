namespace SqlDocStore
{
    using Vendor.EnsureThat;

    public class DocumentStoreSettings
    {
        private string _schema = string.Empty;
        private string _table = "DocumentStore";

        public DocumentStoreSettings(string connectionString)
        {
            EnsureArg.IsNotNullOrWhiteSpace(connectionString);
            ConnectionString = connectionString;
            SchemaCreation = SchemaCreation.Create;
            ConcurrencyModel = ConcurrencyModel.Optimistic;
        }

        public string ConnectionString { get; }

        public string Schema
        {
            get => _schema;
            set
            {
                EnsureArg.IsNotNullOrWhiteSpace(value, nameof(Schema));
                _schema = value;
            }
        }

        public string Table
        {
            get => _table;
            set
            {
                EnsureArg.IsNotNullOrWhiteSpace(value, nameof(Table));
                _table = value;
            }
        }

        public SchemaCreation SchemaCreation { get; set; }

        public ConcurrencyModel ConcurrencyModel { get; set; }
    }
}