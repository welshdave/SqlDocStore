namespace SqlDocStore
{
    using System;

    public interface ITrackedDocument
    {
        object Document { get; set; }
        Type Type { get; set; }
        Guid? ETag { get; set; }
        DocumentState State { get; set; }
    }
}