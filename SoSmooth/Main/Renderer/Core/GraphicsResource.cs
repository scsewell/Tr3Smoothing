using System;
using OpenTK.Graphics;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// A class that controls a handle to an unmanaged graphics resource.
    /// </summary>
    public abstract class GraphicsResource : IDisposable
    {
        protected int m_handle = -1;
        private bool m_disposed = false;

        /// <summary>
        /// The handle to the resource.
        /// </summary>
        public int Handle => m_handle;

        /// <summary>
        /// Indicates if this instance's unmanaged resources are deleted.
        /// </summary>
        public bool Disposed => m_disposed;

        /// <summary>
        /// Deletes all unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Finalizer.
        /// </summary>
        ~GraphicsResource()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }
            if (GraphicsContext.CurrentContext == null)
            {
                Logger.Error("Can't dispose a graphics resource while the current graphics context is null!");
                return;
            }
            if (GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }

            // always clean up unmanaged resources to prevent memory leak.
            OnDispose();

            // cleanup of managed data only matters if not called by the garbage collector finalizer.
            if (disposing)
            {
                m_handle = -1;
            }

            m_disposed = true;
        }

        /// <summary>
        /// Cleanup of unmanaged resources.
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// Casts the graphics resource object to its instance handle
        /// for easy use with graphics API functions.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>The instance's handle.</returns>
        public static implicit operator int(GraphicsResource resource)
        {
            return resource.Handle;
        }
    }
}
