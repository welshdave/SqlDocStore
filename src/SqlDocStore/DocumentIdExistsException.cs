namespace SqlDocStore
{
    using System;

    public class DocumentIdExistsException : Exception
    {
        public DocumentIdExistsException()
        {
        }

        public DocumentIdExistsException(string message) : base(message)
        {
        }

        public DocumentIdExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}