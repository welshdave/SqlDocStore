namespace SqlDocStore
{
    using System;

    public abstract class DisposableBase : IDisposable
    {
        protected bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void CheckNotDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException("{0} has already been disposed", GetType().Name);
        }

        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }

        ~DisposableBase()
        {
            Dispose(false);
        }
    }
}