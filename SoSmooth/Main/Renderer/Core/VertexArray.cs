using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Represents a vertex array object that stores information about 
    /// vertex buffer object attribute layouts.
    /// </summary>
    public class VertexArray<TVertexData> : GraphicsResource where TVertexData : struct, IVertexData
    {
        private readonly VertexBuffer<TVertexData> m_vertexBuffer;
        private bool m_vertexArrayGenerated = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="vertexBuffer">A vertex buffer object to set the attributes for.</param>
        public VertexArray(VertexBuffer<TVertexData> vertexBuffer)
        {
            m_vertexBuffer = vertexBuffer;
        }

        public void SetVertexData()
        {
            GL.BindVertexArray(this);
        }

        public void UnSetVertexData()
        {
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Prepares the vertex attributes for a new shader program.
        /// </summary>
        /// <param name="program">A shader program.</param>
        public void SetShaderProgram(ShaderProgram program)
        {
            if (m_vertexArrayGenerated)
            {
                GL.DeleteVertexArrays(1, ref m_handle);
                m_vertexArrayGenerated = false;
            }

            GL.GenVertexArrays(1, out m_handle);
            m_vertexArrayGenerated = true;

            GL.BindVertexArray(this);
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBuffer);

            program.SetVertexAttributes(new TVertexData().VertexAttributes());

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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
