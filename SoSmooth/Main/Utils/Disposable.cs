using System;

namespace SoSmooth
{
    /// <summary>
    /// A class that contains disposal boiler plate code.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private bool m_disposed = false;
        
        /// <summary>
        /// Indicates if this instance has been disposed.
        /// </summary>
        public bool Disposed => m_disposed;

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">If true managed resources should be cleaned up.</param>
        private void Dispose(bool disposing)
        {
            if (!m_disposed && CanDispose())
            {
                OnDispose(disposing);
                m_disposed = true;
            }
        }

        /// <summary>
        /// Checks if this instance can be disposed.
        /// </summary>
        protected virtual bool CanDispose() { return true; }

        /// <summary>
        /// Cleans up resources held by this instance.
        /// </summary>
        /// <param name="disposing">If true managed resources should be cleaned up.</param>
        protected abstract void OnDispose(bool disposing);
    }
}
