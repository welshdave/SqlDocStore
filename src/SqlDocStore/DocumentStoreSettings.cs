namespace SqlDocStore
{
    using EnsureThat;

    public class DocumentStoreSettings
    {
        private string _schema = string.Empty;
        private string _table = "DocumentStore";

        public DocumentStoreSettings(string connectionString)
        {
            EnsureArg.IsNotNullOrWhiteSpace(connectionString);
            ConnectionString = connectionString;
            SchemaCreation = SchemaCreation.Create;
        }

        public string ConnectionString { get; }

        public string Schema
        {
            get => _schema;
            set
            {
                Ensure.That(value, nameof(Schema)).IsNotNullOrWhiteSpace();
                _schema = value;
            }
        }

        public string Table
        {
            get => _table;
            set
            {
                Ensure.That(value, nameof(Table)).IsNotNullOrWhiteSpace();
                _table = value;
            }
        }

        public SchemaCreation SchemaCreation { get; set; }
    }
}