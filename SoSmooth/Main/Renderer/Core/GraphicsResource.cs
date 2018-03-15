using System;
using OpenTK.Graphics;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// A class that controls a handle to an unmanaged graphics resource.
    /// </summary>
    public abstract class GraphicsResource : Disposable
    {
        protected int m_handle = -1;

        /// <summary>
        /// The handle to the resource.
        /// </summary>
        public int Handle
        {
            get
            {
                ValidateDispose();
                return m_handle;
            }
        }
        
        /// <summary>
        /// Checks if this instance can be disposed.
        /// </summary>
        protected override bool CanDispose()
        {
            if (GraphicsContext.CurrentContext == null)
            {
                Logger.Error($"Can't dispose a graphics resource while the current graphics context is null! Type:{GetType().Name} {ToString()}");
                return false;
            }
            if (GraphicsContext.CurrentContext.IsDisposed)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cleans up resources held by this instance.
        /// </summary>
        /// <param name="disposing">If true managed resources should be cleaned up.</param>
        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                m_handle = -1;
            }
        }

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
