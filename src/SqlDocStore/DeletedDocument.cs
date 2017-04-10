namespace SqlDocStore
{
    public class DeletedDocument
    {
        public DeletedDocument(object id)
        {
            Id = id;
        }
        public object Id { get; }
    }
}
