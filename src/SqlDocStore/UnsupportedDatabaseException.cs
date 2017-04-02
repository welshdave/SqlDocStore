namespace SqlDocStore
{
    using System;

    public class UnsupportedDatabaseException : Exception
    {
        public UnsupportedDatabaseException()
        {
        }

        public UnsupportedDatabaseException(string message) : base(message)
        {
        }

        public UnsupportedDatabaseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}