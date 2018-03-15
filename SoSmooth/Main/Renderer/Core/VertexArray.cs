using System;
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
        private IIndexBuffer m_indexBuffer;
        private bool m_dirty;

        private bool m_vertexArrayGenerated;

        public IVertexBuffer VertexBuffer => m_vertexBuffer;
        public IIndexBuffer IndexBuffer => m_indexBuffer;

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
        /// <param name="vertexBuffer">A vertex buffer object to bind to this object.</param>
        public void SetVertexBuffer(IVertexBuffer vertexBuffer)
        {
            ValidateDispose();

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
        public void SetIndexBuffer(IIndexBuffer indexBuffer)
        {
            ValidateDispose();

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
            ValidateDispose();

            m_program = program;
            m_dirty = true;
        }

        /// <summary>
        /// Binds the vertex array.
        /// </summary>
        public void Bind()
        {
            ValidateDispose();

            UpdateArray();
            GL.BindVertexArray(this);
        }

        /// <summary>
        /// Unbinds the vertex array.
        /// </summary>
        public void Unbind()
        {
            ValidateDispose();

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Sets up a new vertex array object if needing to be updated.
        /// </summary>
        private void UpdateArray()
        {
            if (m_dirty && m_program != null && m_vertexBuffer != null)
            {
                // destroy the old vertex array
                if (m_vertexArrayGenerated)
                {
                    GL.DeleteVertexArray(this);
                    m_vertexArrayGenerated = false;
                }

                // create a new vertex array
                m_handle = GL.GenVertexArray();
                m_vertexArrayGenerated = true;

                // bind the new array
                GL.BindVertexArray(this);

                // set the source vertex and index buffers
                m_vertexBuffer.Bind();
                if (m_indexBuffer != null)
                {
                    m_indexBuffer.Bind();
                }

                // set the vertex attributes
                m_program.SetVertexAttributes(m_vertexBuffer.VertexAttributes);

                // unbind all the buffers
                GL.BindVertexArray(0);
                m_vertexBuffer.Unbind();
                if (m_indexBuffer != null)
                {
                    m_indexBuffer.Unbind();
                }

                m_dirty = false;
            }
        }

        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose(bool disposing)
        {
            if (m_vertexArrayGenerated)
            {
                GL.DeleteVertexArray(this);
            }

            base.OnDispose(disposing);
        }
    }
}
