using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a vertex buffer object that can be rendered with a specified <see cref="BeginMode"/>.
    /// </summary>
    public abstract class VertexSurface : Surface, IDisposable
    {
        /// <summary>
        /// The vertex buffer object containing the vertices to render.
        /// </summary>
        protected IVertexBuffer m_vertexBuffer;

        /// <summary>
        /// The vertex array object.
        /// </summary>
        protected VertexArray m_vertexAttributeProvider; 

        private PrimitiveType m_primitiveType;
        protected PrimitiveType PrimitiveType { get { return m_primitiveType; } }
        
        /// <summary>
        /// Constructs a new <see cref="VertexSurface"/>.
        /// </summary>
        public VertexSurface()
        {
            m_vertexAttributeProvider = new VertexArray();
        }

        /// <summary>
        /// Handles setting up a shader program with this surface.
        /// </summary>
        protected override void OnNewShaderProgram()
        {
            m_vertexAttributeProvider.SetShaderProgram(Program);
        }

        /// <summary>
        /// Sets the vertex buffer object for this surface.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer object to render using.</param>
        /// <param name="primitiveType">Type of the primitives to draw.</param>
        public void SetVertexBuffer(IVertexBuffer vertexBuffer, PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            if (m_vertexBuffer != vertexBuffer)
            {
                m_vertexBuffer = vertexBuffer;
                m_vertexAttributeProvider.SetVertexBuffer(vertexBuffer);
            }
            m_primitiveType = primitiveType;
        }

        /// <summary>
        /// Renderes the vertex buffer.
        /// </summary>
        protected override void OnRender()
        {
            if (m_vertexBuffer.Count == 0)
            {
                return;
            }
            
            m_vertexAttributeProvider.Bind();
            GL.DrawArrays(m_primitiveType, 0, m_vertexBuffer.Count);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Frees unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (m_vertexAttributeProvider != null)
            {
                m_vertexAttributeProvider.Dispose();
            }
        }
    }
}
