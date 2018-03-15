using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a surface rendered using and indexed vertex buffer object.
    /// </summary>
    public class IndexedSurface : VertexSurface
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedSurface"/> class.
        /// </summary>
        public IndexedSurface()
        {
        }

        /// <summary>
        /// Set the index buffer used for this surface.
        /// </summary>
        /// <param name="indexBuffer">The index buffer object to render using.</param>
        public void SetIndexBuffer(IIndexBuffer indexBuffer)
        {
            ValidateDispose();
            m_vertexArray.SetIndexBuffer(indexBuffer);
        }

        /// <summary>
        /// Renders from the index and vertex buffers.
        /// </summary>
        protected override void OnRender()
        {
            IVertexBuffer vertBuf = m_vertexArray.VertexBuffer;
            IIndexBuffer indexBuf = m_vertexArray.IndexBuffer;

            if (vertBuf != null && vertBuf.Count > 0 && indexBuf != null && indexBuf.Count > 0)
            {
                m_vertexArray.Bind();
                GL.DrawElements(PrimitiveType, indexBuf.Count, indexBuf.ElementType, 0);
                m_vertexArray.Unbind();
            }
        }
    }
}
