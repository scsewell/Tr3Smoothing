using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Represents a vertex array object that stores information about 
    /// vertex buffer object attribute layouts.
    /// </summary>
    public class VertexArray : GraphicsResource
    {
        private ShaderProgram m_program;
        private IVertexBuffer m_vertexBuffer;
        private IndexBuffer m_indexBuffer;
        private bool m_vertexArrayGenerated;
        private bool m_dirty;

        public IVertexBuffer VertexBuffer => m_vertexBuffer;
        public IndexBuffer IndexBuffer => m_indexBuffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VertexArray()
        {
            m_vertexArrayGenerated = false;
            m_dirty = true;
        }

        /// <summary>
        /// Sets the vertex buffer paired with this vertex array.
        /// </summary>
        /// <param name="vertexBuffer">A vertex buffer object to set the attributes for.</param>
        public void SetVertexBuffer(IVertexBuffer vertexBuffer)
        {
            if (m_vertexBuffer != vertexBuffer)
            {
                m_vertexBuffer = vertexBuffer;
                m_dirty = true;
            }
        }

        /// <summary>
        /// Sets the index buffer paired with this vertex array.
        /// </summary>
        /// <param name="indexBuffer">An index buffer object to bind to this object.</param>
        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            if (m_indexBuffer != indexBuffer)
            {
                m_indexBuffer = indexBuffer;
                m_dirty = true;
            }
        }

        /// <summary>
        /// Prepares the vertex attributes for a new shader program.
        /// </summary>
        /// <param name="program">A shader program.</param>
        public void SetShaderProgram(ShaderProgram program)
        {
            m_program = program;
            m_dirty = true;
        }

        /// <summary>
        /// Binds the vertex array.
        /// </summary>
        public void Bind()
        {
            UpdateArray();
            GL.BindVertexArray(this);
        }

        /// <summary>
        /// Sets up a new vertex array object if needing to be updated.
        /// </summary>
        private void UpdateArray()
        {
            if (m_dirty && m_program != null && m_vertexBuffer != null)
            {
                if (m_vertexArrayGenerated)
                {
                    GL.DeleteVertexArray(this);
                    m_vertexArrayGenerated = false;
                }

                m_handle = GL.GenVertexArray();
                m_vertexArrayGenerated = true;

                GL.BindVertexArray(this);
                m_vertexBuffer.Bind(BufferTarget.ArrayBuffer);

                if (m_indexBuffer != null)
                {
                    m_indexBuffer.Bind();
                }

                m_program.SetVertexAttributes(m_vertexBuffer.VertexAttributes);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                m_dirty = false;
            }
        }

        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose()
        {
            if (m_vertexArrayGenerated)
            {
                GL.DeleteVertexArray(this);
            }
        }
    }
}
