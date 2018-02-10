using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderering
{
    /// <summary>
    /// This class represents an OpenGL index buffer object.
    /// </summary>
    /// <remarks>Note that this object can hold no more than 2^31 indices, and that indices are stored as 16 bit unsigned integer.</remarks>
    public sealed class IndexBuffer : IDisposable
    {
        private readonly int m_handle;
        private ushort[] m_indices;
        private int m_count;
        
        /// <summary>
        /// The OpenGL index buffer object handle.
        /// </summary>
        public int Handle { get { return m_handle; } }

        /// <summary>
        /// The number of indices in this buffer.
        /// </summary>
        public int Count { get { return m_count; } }

        /// <summary>
        /// The size of the underlying array of indices.
        /// The array resizes automatically, when more indices are added.
        /// </summary>
        public int Capacity { get { return m_indices.Length; } }
        
        /// <summary>
        /// Initialises a new instance of <see cref="IndexBuffer"/>.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public IndexBuffer(int capacity = 0)
        {
            m_indices = new ushort[capacity > 0 ? capacity : 1];
            m_handle = GL.GenBuffer();
        }
        
        private void EnsureCapacity(int minCapacity)
        {
            if (m_indices.Length <= minCapacity)
            {
                Array.Resize(ref m_indices, Math.Max(m_indices.Length * 2, minCapacity));
            }
        }
        
        /// <summary>
        /// Adds an index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void AddIndex(ushort index)
        {
            if (m_indices.Length == m_count)
            {
                Array.Resize(ref m_indices, m_indices.Length * 2);
            }
            m_indices[m_count] = index;
        }

        /// <summary>
        /// Adds indices.
        /// </summary>
        public void AddIndices(ushort index0, ushort index1)
        {
            int newCount = m_count + 2;
            EnsureCapacity(newCount);
            m_indices[m_count] = index0;
            m_indices[m_count + 1] = index1;
            m_count = newCount;
        }
        
        /// <summary>
        /// Adds indices.
        /// </summary>
        public void AddIndices(ushort index0, ushort index1, ushort index2)
        {
            int newCount = m_count + 3;
            EnsureCapacity(newCount);
            m_indices[m_count] = index0;
            m_indices[m_count + 1] = index1;
            m_indices[m_count + 2] = index2;
            m_count = newCount;
        }

        /// <summary>
        /// Adds indices.
        /// </summary>
        /// <param name="indices">The indices.</param>
        public void AddIndices(params ushort[] indices)
        {
            int newCount = m_count + indices.Length;
            EnsureCapacity(newCount);
            Array.Copy(indices, 0, m_indices, m_count, indices.Length);
            m_count = newCount;
        }

        /// <summary>
        /// Exposes the underlying array of the index buffer directly,
        /// to allow for faster index creation.
        /// </summary>
        /// <param name="count">The amount of indices to write.
        /// The returned array is guaranteed to have this much space.
        /// Do not write more than this number of indices.
        /// Note also that writing less indices than specified may result in undefined behaviour.</param>
        /// <param name="offset">The offset of the first index to write to.</param>
        /// <remarks>Write indices to the array in the full range [offset, offset + count[.
        /// Writing more or less or outside that range may result in undefined behaviour.</remarks>
        /// <returns>The underlying array of indices to write to. This array is only valid for this single call.
        /// To copy more indices, call this method again and use the new return value.</returns>
        public ushort[] WriteIndicesDirectly(int count, out int offset)
        {
            int newCount = m_count + count;
            EnsureCapacity(newCount);
            offset = m_count;
            m_count = newCount;
            return m_indices;
        }
        
        /// <summary>
        /// Removes the last <paramref name="count"/> indices.
        /// </summary>
        /// <param name="count"></param>
        public void RemoveIndices(int count)
        {
            m_count = count > m_count ? 0 : (m_count - count);
        }

        /// <summary>
        /// Clears the index buffer.
        /// </summary>
        public void Clear()
        {
            m_count = 0;
        }
        
        /// <summary>
        /// Binds the index buffer.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Bind(BufferTarget target = BufferTarget.ElementArrayBuffer)
        {
            GL.BindBuffer(target, this);
        }

        /// <summary>
        /// Uploads the index buffer to the GPU.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="usageHint">The usage hint.</param>
        public void BufferData(BufferTarget target = BufferTarget.ElementArrayBuffer, BufferUsageHint usageHint = BufferUsageHint.StreamDraw)
        {
            GL.BufferData(target, (IntPtr)(sizeof(ushort) * m_count), m_indices, usageHint);
        }

        /// <summary>
        /// Implicit cast to int for easy usage in GL methods that require an integer handle.
        /// </summary>
        public static implicit operator int(IndexBuffer buffer)
        {
            return buffer.m_handle;
        }

        private bool m_disposed = false;

        /// <summary>
        /// Disposes the buffer, to make sure GL resources are freed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~IndexBuffer()
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
