using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderering
{
    /// <summary>
    /// This class represents and OpenGL vertex buffer object.
    /// </summary>
    /// <remarks>Note that this object can hold no more than 2^16 vertices.</remarks>
    /// <typeparam name="TVertexData">The type of vertex in the buffer.</typeparam>
    public sealed class VertexBuffer<TVertexData> : IDisposable where TVertexData : struct, IVertexData
    {
        private readonly int m_handle;
        private readonly int m_vertexSize;

        private TVertexData[] m_vertices;
        private ushort m_count;
        
        /// <summary>
        /// The OpenGL vertex buffer object handle.
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
        /// The size of the underlying array of vertices.
        /// The array resizes automatically, when more vertices are added.
        /// </summary>
        public int Capacity { get { return m_vertices.Length; } }
        
        /// <summary>
        /// Initialises a new instance of <see cref="VertexBuffer{TVertexData}"/>.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public VertexBuffer(int capacity = 0)
        {
            m_vertexSize = VertexData.SizeOf<TVertexData>();
            m_vertices = new TVertexData[capacity > 0 ? capacity : 4];
            m_handle = GL.GenBuffer();
        }
        
        private void EnsureCapacity(int minCapacity)
        {
            if (m_vertices.Length <= minCapacity)
            {
                Array.Resize(ref m_vertices, Math.Max(m_vertices.Length * 2, minCapacity));
            }
        }
        
        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>Index of the vertex in vertex buffer.</returns>
        public ushort AddVertex(TVertexData vertex)
        {
            if (m_vertices.Length == m_count)
            {
                Array.Resize(ref m_vertices, m_vertices.Length * 2);
            }
            m_vertices[m_count] = vertex;
            return m_count++;
        }

        /// <summary>
        /// Adds two vertices.
        /// </summary>
        /// <returns>Index of first new vertex in vertex buffer.</returns>
        public ushort AddVertices(TVertexData vertex0, TVertexData vertex1)
        {
            ushort oldCount = m_count;
            int newCount = oldCount + 2;
            EnsureCapacity(newCount);
            m_count = (ushort)newCount;

            m_vertices[oldCount] = vertex0;
            m_vertices[oldCount + 1] = vertex1;

            return oldCount;
        }

        /// <summary>
        /// Adds three vertices.
        /// </summary>
        /// <returns>Index of first new vertex in vertex buffer.</returns>
        public ushort AddVertices(TVertexData vertex0, TVertexData vertex1, TVertexData vertex2)
        {
            ushort oldCount = m_count;
            int newCount = oldCount + 3;
            EnsureCapacity(newCount);
            m_count = (ushort)newCount;

            m_vertices[oldCount] = vertex0;
            m_vertices[oldCount + 1] = vertex1;
            m_vertices[oldCount + 2] = vertex2;

            return oldCount;
        }

        /// <summary>
        /// Adds four vertices.
        /// </summary>
        /// <returns>Index of first new vertex in vertex buffer.</returns>
        public ushort AddVertices(TVertexData vertex0, TVertexData vertex1, TVertexData vertex2, TVertexData vertex3)
        {
            ushort oldCount = m_count;
            int newCount = oldCount + 4;
            EnsureCapacity(newCount);
            m_count = (ushort)newCount;

            m_vertices[oldCount] = vertex0;
            m_vertices[oldCount + 1] = vertex1;
            m_vertices[oldCount + 2] = vertex2;
            m_vertices[oldCount + 3] = vertex3;

            return oldCount;
        }

        /// <summary>
        /// Adds vertices.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <returns>Index of first new vertex in vertex buffer.</returns>
        public ushort AddVertices(params TVertexData[] vertices)
        {
            ushort oldCount = m_count;
            int newCount = oldCount + vertices.Length;
            EnsureCapacity(newCount);
            Array.Copy(vertices, 0, m_vertices, m_count, vertices.Length);
            m_count = (ushort)newCount;
            return oldCount;
        }

        /// <summary>
        /// Exposes the underlying array of the vertex buffer directly,
        /// to allow for faster vertex creation.
        /// </summary>
        /// <param name="count">The amount of vertices to write.
        /// The returned array is guaranteed to have this much space.
        /// Do not write more than this number of vertices.
        /// Note also that writing less vertices than specified may result in undefined behaviour.</param>
        /// <param name="offset">The offset of the first vertex to write to.</param>
        /// <remarks>Write vertices to the array in the full range [offset, offset + count[.
        /// Writing more or less or outside that range may result in undefined behaviour.</remarks>
        /// <returns>The underlying array of vertices to write to. This array is only valid for this single call.
        /// To copy more vertices, call this method again and use the new return value.</returns>
        public TVertexData[] WriteVerticesDirectly(int count, out ushort offset)
        {
            ushort oldCount = m_count;
            int newCount = oldCount + count;

            EnsureCapacity(newCount);

            m_count = (ushort)newCount;

            offset = oldCount;
            return m_vertices;
        }
        
        /// <summary>
        /// Removes a the last <paramref name="count"/> vertices added.
        /// </summary>
        public void RemoveVertices(int count)
        {
            m_count = count > m_count ? (ushort)0 : (ushort)(m_count - count);
        }

        /// <summary>
        /// Clears the vertex buffer.
        /// </summary>
        public void Clear()
        {
            m_count = 0;
        }
        
        /// <summary>
        /// Binds the vertex buffer object.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Bind(BufferTarget target = BufferTarget.ArrayBuffer)
        {
            GL.BindBuffer(target, this);
        }

        /// <summary>
        /// Uploads the vertex buffer to the GPU.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="usageHint">The usage hint.</param>
        public void BufferData(BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint usageHint = BufferUsageHint.StreamDraw)
        {
            GL.BufferData(target, (IntPtr)(m_vertexSize * m_count), m_vertices, usageHint);
        }

        /// <summary>
        /// Reserve a number of bytes in GPU memory for this vertex buffer.
        /// </summary>
        /// <remarks>
        /// This is useful for advanced features like transform feedback.
        /// </remarks>
        /// <param name="vertexCount">The amount of vertices reserved.</param>
        /// <param name="target">The target.</param>
        /// <param name="usageHint">The usage hint.</param>
        /// <param name="setVertexCount">Whether to set the given vertex count as size of this vertex buffer.</param>
        public void BufferNoData(int vertexCount, BufferTarget target = BufferTarget.ArrayBuffer,
            BufferUsageHint usageHint = BufferUsageHint.StreamDraw, bool setVertexCount = false)
        {
            GL.BufferData(target, (IntPtr)(m_vertexSize * vertexCount), IntPtr.Zero, usageHint);
            if (setVertexCount)
            {
                m_count = (ushort)vertexCount;
            }
        }

        /// <summary>
        /// Implicit cast to easily use vertex buffers in GL functions expecting an integer handle.
        /// </summary>
        public static implicit operator int(VertexBuffer<TVertexData> buffer)
        {
            return buffer.m_handle;
        }
        
        private bool m_disposed = false;

        /// <summary>
        /// Disposes the vertex buffer and deletes the underlying GL object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VertexBuffer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }
            if (GraphicsContext.CurrentContext == null || GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }

            GL.DeleteBuffer(this);

            m_disposed = true;
        }
    }
}
