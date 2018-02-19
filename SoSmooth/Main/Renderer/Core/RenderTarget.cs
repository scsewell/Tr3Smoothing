using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents an OpenGL framebuffer object that can be rendered to.
    /// </summary>
    public sealed class RenderTarget : GraphicsResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> class.
        /// </summary>
        public RenderTarget()
        {
            m_handle = GL.GenFramebuffer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> class assigning a texture to its default color attachment.
        /// </summary>
        /// <param name="texture">The texture to attach.</param>
        public RenderTarget(Texture texture)
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
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this);
            GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, attachment, target, texture, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }
        
        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose()
        {
            GL.DeleteFramebuffer(m_handle);
        }
    }
}
