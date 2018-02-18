using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// The base for all buffer objects. Manages an array that contains all elements
    /// to be buffered on the GPU. A count is used to keep track of how many elements 
    /// to use, much like an array list, mimimizing allocations.
    /// </summary>
    public abstract class Buffer<DataType> : GraphicsResource where DataType : struct
    {
        /// <summary>
        /// A mapping that stores the size in bytes of any types that have been used in a buffer.
        /// </summary>
        private static Dictionary<Type, int> m_typeSizes = new Dictionary<Type, int>();
        
        protected DataType[] m_buffer;
        protected int m_count;

        private readonly BufferTarget m_target;
        private readonly int m_elementSize;

        /// <summary>
        /// The number of elements in the buffer.
        /// </summary>
        public int Count => m_count;

        /// <summary>
        /// Initialises a new <see cref="Buffer{DataType}"/> instance.
        /// </summary>
        /// <param name="target">The buffer type.</param>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public Buffer(BufferTarget target, int capacity = 1)
        {
            m_handle = GL.GenBuffer();
            m_target = target;

            m_buffer = new DataType[capacity];

            // get the size in bytes of each element in the buffer array
            Type dataType = typeof(DataType);
            if (!m_typeSizes.TryGetValue(dataType, out m_elementSize))
            {
                // if this type has not been used in a buffer yet, compute the element size
                m_elementSize = Marshal.SizeOf(dataType);
                m_typeSizes.Add(dataType, m_elementSize);
            }
        }

        /// <summary>
        /// Binds the buffer object.
        /// </summary>
        public void Bind()
        {
            GL.BindBuffer(m_target, this);
        }

        /// <summary>
        /// Unbinds the buffer object.
        /// </summary>
        public void Unbind()
        {
            GL.BindBuffer(m_target, this);
        }

        /// <summary>
        /// Uploads the buffer to the GPU.
        /// </summary>
        /// <param name="usageHint">The usage hint.</param>
        public void BufferData(BufferUsageHint usageHint = BufferUsageHint.DynamicDraw)
        {
            GL.BindBuffer(m_target, this);
            GL.BufferData(m_target, m_elementSize * m_count, m_buffer, usageHint);
            GL.BindBuffer(m_target, 0);
        }

        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose()
        {
            GL.DeleteBuffer(this);
        }
    }
}
