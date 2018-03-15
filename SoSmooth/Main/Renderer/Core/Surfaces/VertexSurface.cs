using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a surface rendered using a vertex buffer object.
    /// </summary>
    public class VertexSurface : Surface
    {
        /// <summary>
        /// The vertex array object.
        /// </summary>
        protected VertexArray m_vertexArray; 

        private PrimitiveType m_primitiveType;
        protected PrimitiveType PrimitiveType => m_primitiveType;
        
        /// <summary>
        /// Constructs a new <see cref="VertexSurface"/>.
        /// </summary>
        public VertexSurface()
        {
            m_vertexArray = new VertexArray();
        }

        /// <summary>
        /// Handles setting up a shader program with this surface.
        /// </summary>
        protected override void OnNewShaderProgram()
        {
            m_vertexArray.SetShaderProgram(Program);
        }

        /// <summary>
        /// Sets the vertex buffer object for this surface.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer object to render using.</param>
        /// <param name="primitiveType">Type of the primitives to draw.</param>
        public void SetVertexBuffer(IVertexBuffer vertexBuffer, PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            ValidateDispose();
            m_vertexArray.SetVertexBuffer(vertexBuffer);
            m_primitiveType = primitiveType;
        }

        /// <summary>
        /// Renderes the vertex buffer.
        /// </summary>
        protected override void OnRender()
        {
            IVertexBuffer vertBuf = m_vertexArray.VertexBuffer;

            if (vertBuf != null && vertBuf.Count > 0)
            {
                m_vertexArray.Bind();
                GL.DrawArrays(m_primitiveType, 0, vertBuf.Count);
                m_vertexArray.Unbind();
            }
        }

        /// <summary>
        /// Frees unmanaged resources.
        /// </summary>
        protected override void OnDispose(bool disposing)
        {
            if (m_vertexArray != null)
            {
                m_vertexArray.Dispose();
            }
        }
    }
}
