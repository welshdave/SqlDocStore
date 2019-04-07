namespace SqlDocStore
{
    using System;
    
    public class InvalidDocumentException : Exception
    {
        public InvalidDocumentException()
        {
        }

        public InvalidDocumentException(string message) : base(message)
        {
        }

        public InvalidDocumentException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
