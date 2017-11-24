using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Buffers Vertices in a format that can be transfered to the GPU.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex stored in the buffer.</typeparam>
    sealed public class VertexBuffer<TVertex> : IDisposable where TVertex : struct
    {
        private readonly int m_handle;
        private readonly int m_vertexSize;
        private TVertex[] m_vertices;
        private ushort m_count;
        private bool m_disposed;

        /// <summary>
        /// The vertex buffer object handle.
        /// </summary>
        public int Handle { get { return m_handle; } }

        /// <summary>
        /// The size of a vertex in bytes.
        /// </summary>
        public int VertexSize { get { return m_vertexSize; } }

        /// <summary>
        /// The number of vertices in this VertexBuffer.
        /// </summary>
        public ushort Count { get { return m_count; } }

        /// <summary>
        /// Constructs a new vertex buffer.
        /// </summary>
        /// <param name="vertexSize"></param>
        public VertexBuffer(int vertexSize)
        {
            m_vertexSize = vertexSize;
            m_handle = GL.GenBuffer();

            m_vertices = new TVertex[4];
            m_count = 0;
            m_disposed = false;
        }

        /// <summary>
        /// Adds a vertex to the buffer.
        /// </summary>
        /// <param name="v">The vertex to add.</param>
        public void AddVertex(TVertex v)
        {
            if (m_count == m_vertices.Length)
            {
                Array.Resize(ref m_vertices, m_count * 2);
            }
            m_vertices[m_count] = v;
            m_count++;
        }

        /// <summary>
        /// Make this the active array buffer.
        /// </summary>
        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_handle);
        }

        /// <summary>
        /// Copy contained vertices to GPU memory.
        /// </summary>
        public void BufferData()
        {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(m_vertexSize * m_count), m_vertices, BufferUsageHint.StreamDraw);
        }

        /// <summary>
        /// Draw buffered vertices as triangles.
        /// </summary>
        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, m_count);
        }

        public void Dispose()
        {
            FreeBuffer();
            GC.SuppressFinalize(this);
        }

        ~VertexBuffer()
        {
            FreeBuffer();
        }

        private void FreeBuffer()
        {
            if (!m_disposed && GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed)
            {
                GL.DeleteBuffer(m_handle);
                m_disposed = true;
            }
        }
    }
}
