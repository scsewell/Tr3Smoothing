using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderer.Meshes
{
    /// <summary>
    /// This class represents an OpenGL framebuffer object that can be rendered to.
    /// </summary>
    public sealed class RenderTarget : IDisposable
    {
        private readonly int m_handle;
        
        /// <summary>
        /// The handle of the OpenGL framebuffer object associated with this render target
        /// </summary>
        public int Handle { get { return m_handle; } }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> class.
        /// </summary>
        public RenderTarget()
        {
            GL.GenFramebuffers(1, out m_handle);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> class assigning a texture to its default color attachment.
        /// </summary>
        /// <param name="texture">The texture to attach.</param>
        public RenderTarget(Texture texture) : this()
        {
            Attach(FramebufferAttachment.ColorAttachment0, texture);
        }
        
        /// <summary>
        /// Attaches a <see cref="Texture"/> to the specified <see cref="FramebufferAttachment"/>, so it can be rendered to.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="target">Texture target of the attachment.</param>
        public void Attach(FramebufferAttachment attachment, Texture texture, TextureTarget target = TextureTarget.Texture2D)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, m_handle);
            GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, attachment, target, texture, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }
        
        /// <summary>
        /// Casts the <see cref="RenderTarget"/> to its OpenGL framebuffer object handle, for easy use with OpenGL functions.
        /// </summary>
        /// <remarks>Null is cast to 0.</remarks>
        static public implicit operator int(RenderTarget renderTarget)
        {
            if (renderTarget == null)
            {
                return 0;
            }
            return renderTarget.Handle;
        }
        
        private bool m_disposed = false;

        /// <summary>
        /// Disposes of the render target and deletes the underlying GL object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RenderTarget()
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

            int handle = Handle;
            GL.DeleteFramebuffers(1, ref handle);

            m_disposed = true;
        }
    }
}
