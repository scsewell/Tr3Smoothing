using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents an indexed vertex buffer object, that can be draws with a specified <see cref="BeginMode"/>.
    /// </summary>
    public class IndexedSurface : VertexSurface
    {
        /// <summary>
        /// The index buffer object container the rendered indices.
        /// </summary>
        protected IndexBuffer m_indexBuffer;

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
            if (m_indexBuffer != indexBuffer)
            {
                m_indexBuffer = indexBuffer;
            }
        }

        /// <summary>
        /// Renders from the index and vertex buffers.
        /// </summary>
        protected override void OnRender()
        {
            if (m_indexBuffer.Count == 0 || m_vertexBuffer.Count == 0)
            {
                return;
            }
            
            m_vertexAttributeProvider.Bind();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indexBuffer);
            
            GL.DrawElements(PrimitiveType, m_indexBuffer.Count, DrawElementsType.UnsignedShort, 0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
