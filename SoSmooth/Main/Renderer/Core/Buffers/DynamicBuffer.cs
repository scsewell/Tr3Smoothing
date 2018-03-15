using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Adds helper methods to the base buffer class to help with adding and removing
    /// elements.
    /// </summary>
    public abstract class DynamicBuffer<TData> : Buffer<TData> where TData : struct
    {
        /// <summary>
        /// Initialises a new <see cref="DynamicBuffer{DataType}"/> instance.
        /// </summary>
        /// <param name="target">The buffer type.</param>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public DynamicBuffer(BufferTarget target, int capacity) : base(target, capacity)
        {
        }

        /// <summary>
        /// Ensures the buffer array is large enough to store a given number of elements.
        /// </summary>
        /// <param name="minCapacity">The number of elements the buffer must be able to contain.</param>
        private void EnsureCapacity(int minCapacity)
        {
            if (m_buffer.Length < minCapacity)
            {
                Array.Resize(ref m_buffer, Math.Max(m_buffer.Length * 2, minCapacity));
            }
        }

        /// <summary>
        /// Adds an element to the buffer.
        /// </summary>
        /// <param name="element">The element to add to the buffer.</param>
        public void AddElement(TData element)
        {
            ValidateDispose();

            int newCount = m_count + 1;
            EnsureCapacity(newCount);

            m_buffer[m_count] = element;

            m_count = newCount;
            m_dirty = true;
        }

        /// <summary>
        /// Adds elements to the buffer.
        /// </summary>
        /// <param name="e0">The first element to add to the buffer.</param>
        /// <param name="e1">The second element to add to the buffer.</param>
        public void AddElements(TData e0, TData e1)
        {
            ValidateDispose();

            int newCount = m_count + 2;
            EnsureCapacity(newCount);

            m_buffer[m_count] = e0;
            m_buffer[m_count + 1] = e1;

            m_count = newCount;
            m_dirty = true;
        }

        /// <summary>
        /// Adds elements to the buffer.
        /// </summary>
        /// <param name="e0">The first element to add to the buffer.</param>
        /// <param name="e1">The second element to add to the buffer.</param>
        /// <param name="e2">The third element to add to the buffer.</param>
        public void AddElements(TData e0, TData e1, TData e2)
        {
            ValidateDispose();

            int newCount = m_count + 3;
            EnsureCapacity(newCount);

            m_buffer[m_count] = e0;
            m_buffer[m_count + 1] = e1;
            m_buffer[m_count + 2] = e2;

            m_count = newCount;
            m_dirty = true;
        }

        /// <summary>
        /// Adds elements to the buffer.
        /// </summary>
        /// <param name="e0">The first element to add to the buffer.</param>
        /// <param name="e1">The second element to add to the buffer.</param>
        /// <param name="e2">The third element to add to the buffer.</param>
        /// <param name="e3">The fourth element to add to the buffer.</param>
        public void AddElements(TData e0, TData e1, TData e2, TData e3)
        {
            ValidateDispose();

            int newCount = m_count + 4;
            EnsureCapacity(newCount);

            m_buffer[m_count] = e0;
            m_buffer[m_count + 1] = e1;
            m_buffer[m_count + 2] = e2;
            m_buffer[m_count + 3] = e3;

            m_count = newCount;
            m_dirty = true;
        }

        /// <summary>
        /// Adds elements to the buffer.
        /// </summary>
        /// <param name="elements">The elements to add to the buffer.</param>
        public void AddElements(params TData[] elements)
        {
            ValidateDispose();

            int newCount = m_count + elements.Length;
            EnsureCapacity(newCount);

            Array.Copy(elements, 0, m_buffer, m_count, elements.Length);

            m_count = newCount;
            m_dirty = true;
        }

        /// <summary>
        /// Exposes the underlying array of the buffer directly,
        /// to allow for faster filling.
        /// </summary>
        /// <param name="count">The amount of additional elements to be written.
        /// The returned array is guaranteed to have enough space for the original
        /// elements and count new elements.</param>
        /// <param name="offset">The offset of the first index to write to.</param>
        /// <remarks>Write elements to the array in the indices [offset, offset + count].
        /// Writing outside that range may result in undefined behaviour.</remarks>
        /// <returns>The underlying element array to write to.</returns>
        public TData[] WriteDirectly(int count, out int offset)
        {
            ValidateDispose();

            int newCount = m_count + count;
            EnsureCapacity(newCount);
            offset = m_count;

            m_count = newCount;
            m_dirty = true;
            return m_buffer;
        }

        /// <summary>
        /// Removes the last <paramref name="count"/> elements from the end of the buffer.
        /// </summary>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveElements(int count)
        {
            ValidateDispose();

            m_count = Math.Max(m_count - count, 0);
            m_dirty = true;
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear()
        {
            ValidateDispose();

            m_count = 0;
            m_dirty = true;
        }
    }
}
