using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a surface rendered using a vertex buffer object.
    /// </summary>
    public abstract class VertexSurface : Surface, IDisposable
    {
        /// <summary>
        /// The vertex array object.
        /// </summary>
        protected VertexArray m_vertexArray; 

        private PrimitiveType m_primitiveType;
        protected PrimitiveType PrimitiveType { get { return m_primitiveType; } }
        
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
            m_vertexArray.SetVertexBuffer(vertexBuffer);
            m_primitiveType = primitiveType;
        }

        /// <summary>
        /// Renderes the vertex buffer.
        /// </summary>
        protected override void OnRender()
        {
            if (m_vertexArray.VertexBuffer != null && m_vertexArray.VertexBuffer.Count > 0)
            {
                m_vertexArray.Bind();
                GL.DrawArrays(m_primitiveType, 0, m_vertexArray.VertexBuffer.Count);
                GL.BindVertexArray(0);
            }
        }

        /// <summary>
        /// Frees unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (m_vertexArray != null)
            {
                m_vertexArray.Dispose();
            }
        }
    }
}
