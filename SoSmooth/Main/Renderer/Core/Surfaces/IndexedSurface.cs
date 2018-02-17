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
        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            m_vertexArray.SetIndexBuffer(indexBuffer);
        }

        /// <summary>
        /// Renders from the index and vertex buffers.
        /// </summary>
        protected override void OnRender()
        {
            if (m_vertexArray.VertexBuffer != null && m_vertexArray.VertexBuffer.Count > 0 &&
                m_vertexArray.IndexBuffer != null && m_vertexArray.IndexBuffer.Count > 0)
            {
                m_vertexArray.Bind();
                GL.DrawElements(PrimitiveType, m_vertexArray.IndexBuffer.Count, DrawElementsType.UnsignedShort, 0);
                GL.BindVertexArray(0);
            }
        }
    }
}
