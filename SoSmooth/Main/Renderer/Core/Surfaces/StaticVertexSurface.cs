using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderer
{
    /// <summary>
    /// This class represents a vertex buffer object that can be rendered with a specified <see cref="BeginMode"/>.
    /// </summary>
    /// <typeparam name="TVertexData">The <see cref="IVertexData"/> used for the vertex buffer object</typeparam>
    public abstract class StaticVertexSurface<TVertexData> : Surface where TVertexData : struct, IVertexData
    {
        /// <summary>
        /// The OpenGL vertex buffer containing the rendered vertices.
        /// </summary>
        protected VertexBuffer<TVertexData> m_vertexBuffer;

        /// <summary>
        /// The OpenGL vertex array object handle.
        /// </summary>
        protected IVertexAttributeProvider<TVertexData> m_vertexAttributeProvider; 

        private readonly PrimitiveType m_primitiveType;
        protected PrimitiveType primitiveType { get { return m_primitiveType; } }

        /// <summary>
        /// Whether the vertex buffer object is assumed to be static.
        /// Static buffers will be uploaded to the GPU only once, non-static buffers will be uploaded every draw call.
        /// </summary>
        protected bool m_isStatic = true;

        /// <summary>
        /// Whether the static vertex buffer was already uploaded to the GPU.
        /// </summary>
        protected bool m_staticBufferUploaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticVertexSurface{TVertexData}"/> class.
        /// </summary>
        /// <param name="primitiveType">Type of the primitives to draw</param>
        public StaticVertexSurface(PrimitiveType primitiveType = PrimitiveType.Triangles)
        {
            m_primitiveType = primitiveType;
            m_vertexBuffer = new VertexBuffer<TVertexData>();
            m_vertexAttributeProvider = new VertexArray<TVertexData>(m_vertexBuffer);
        }

        /// <summary>
        /// Handles setting up (new) shader program with this surface.
        /// Calls <see cref="setVertexAttributes"/>.
        /// </summary>
        protected override void OnNewShaderProgram()
        {
            m_vertexAttributeProvider.SetShaderProgram(m_program);
        }

        /// <summary>
        /// Renderes the vertex buffer.
        /// Does so by binding it and the vertex array object, uploading the vertices to the GPU (if first call or is not static), drawing with specified <see cref="BeginMode"/> and unbinding buffers afterwards.
        /// </summary>
        protected override void OnRender()
        {
            if (m_vertexBuffer.Count == 0)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBuffer);
            m_vertexAttributeProvider.SetVertexData();

            bool upload = true;
            if (m_isStatic)
            {
                if (m_staticBufferUploaded)
                {
                    upload = false;
                }
                else
                {
                    m_staticBufferUploaded = true;
                }
            }

            if (upload)
            {
                m_vertexBuffer.BufferData();
            }

            GL.DrawArrays(m_primitiveType, 0, m_vertexBuffer.Count);

            m_vertexAttributeProvider.UnSetVertexData();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Clears the vertex buffer.
        /// </summary>
        public virtual void Clear()
        {
            m_vertexBuffer.Clear();
            m_staticBufferUploaded = false;
        }

        /// <summary>
        /// Forces vertex buffer upload next draw call, even if <see cref="IsStatic"/> is set to true.
        /// </summary>
        public virtual void ForceBufferUpload()
        {
            m_staticBufferUploaded = false;
        }
    }
}
