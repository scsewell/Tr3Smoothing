using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Represents a vertex array object that stores information about 
    /// vertex buffer object attribute layouts.
    /// </summary>
    public class VertexArray : GraphicsResource
    {
        private IVertexBuffer m_vertexBuffer;
        private bool m_vertexArrayGenerated;

        private ShaderProgram m_program;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VertexArray()
        {
            m_vertexArrayGenerated = false;
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
                RegenerateArray();
            }
        }
        
        /// <summary>
        /// Prepares the vertex attributes for a new shader program.
        /// </summary>
        /// <param name="program">A shader program.</param>
        public void SetShaderProgram(ShaderProgram program)
        {
            m_program = program;
            RegenerateArray();
        }

        /// <summary>
        /// Sets up a new vertex array object if ready.
        /// </summary>
        public void RegenerateArray()
        {
            if (m_program != null && m_vertexBuffer != null)
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

                m_program.SetVertexAttributes(m_vertexBuffer.VertexAttributes);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }

        /// <summary>
        /// Binds the vertex array.
        /// </summary>
        public void Bind()
        {
            GL.BindVertexArray(this);
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
