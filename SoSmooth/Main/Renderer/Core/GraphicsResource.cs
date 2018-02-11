using System;
using OpenTK.Graphics;

namespace SoSmooth.Rendering
{
    public abstract class GraphicsResource : IDisposable
    {
        protected int m_handle;

        /// <summary>
        /// The handle to the resource.
        /// </summary>
        public int Handle
        {
            get { return m_handle; }
        }

        private bool m_disposed = false;

        /// <summary>
        /// Indicates if this instance's unmanaged resources are deleted.
        /// </summary>
        public bool Disposed
        {
            get { return m_disposed; }
        }

        /// <summary>
        /// Deletes all unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
            if (GraphicsContext.CurrentContext == null || GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }
            OnDispose();
            m_disposed = true;
        }

        /// <summary>
        /// Cleanup of unmanaged resources.
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// Casts the graphics resource object to its OpenGL instance handle
        /// for easy use with OpenGL functions.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>The instance's handle.</returns>
        public static implicit operator int(GraphicsResource resource)
        {
            return resource.Handle;
        }
    }
}
