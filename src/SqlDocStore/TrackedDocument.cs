namespace SqlDocStore
{
    using System;

    public class TrackedDocument : ITrackedDocument
    {
        public object Document { get; set; }

        public Type Type { get; set; }
        public Guid? ETag { get; set; }

        public DocumentState State { get; set; }
    }
}
