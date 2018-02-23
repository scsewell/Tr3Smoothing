using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// The base for all buffer objects. Manages an array that contains all elements
    /// to be buffered on the GPU. A count is used to keep track of how many elements 
    /// to use, much like an array list, mimimizing allocations.
    /// </summary>
    public abstract class Buffer<TData> : GraphicsResource where TData : struct
    {
        protected static readonly int m_elementSize = Marshal.SizeOf(typeof(TData));

        private readonly BufferTarget m_target;
        private int m_capacity;

        protected TData[] m_buffer;
        protected int m_count;
        protected bool m_dirty;
        
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
            m_target = target;

            m_handle = GL.GenBuffer();
            m_capacity = 0;

            m_buffer = new TData[capacity];
            m_count = 0;
            m_dirty = true;
        }

        /// <summary>
        /// Binds the buffer object.
        /// </summary>
        public void Bind()
        {
            if (!Disposed)
            {
                GL.BindBuffer(m_target, this);
            }
            else
            {
                Logger.Error($"Attepted to bind disposed buffer: " + this);
            }
        }

        /// <summary>
        /// Unbinds the buffer object.
        /// </summary>
        public void Unbind()
        {
            if (!Disposed)
            {
                GL.BindBuffer(m_target, 0);
            }
            else
            {
                Logger.Error($"Attepted to unbind disposed buffer: " + this);
            }
        }

        /// <summary>
        /// Uploads the buffer to the GPU if it has changed since last buffered.
        /// </summary>
        /// <param name="usageHint">The usage hint.</param>
        public void BufferData(BufferUsageHint usageHint = BufferUsageHint.DynamicDraw)
        {
            if (!Disposed)
            {
                if (m_dirty)
                {
                    GL.BindBuffer(m_target, this);

                    // If the allocated buffer on the GPU is large enough, don't reallocate
                    int requiredSize = m_elementSize * m_count;
                    if (m_capacity >= requiredSize)
                    {
                        GL.BufferSubData(m_target, (IntPtr)0, requiredSize, m_buffer);
                    }
                    else
                    {
                        GL.BufferData(m_target, requiredSize, m_buffer, usageHint);
                        m_capacity = requiredSize;
                    }
                    m_dirty = false;

                    GL.BindBuffer(m_target, 0);
                }
            }
            else
            {
                Logger.Error($"Attepted to buffer data from disposed buffer: " + ToString());
            }
        }

        /// <summary>
        /// Gets a string describing this buffer.
        /// </summary>
        public override string ToString()
        {
            return $"{{{GetType().Name}<{typeof(TData).Name}> Handle:{m_handle} ElementSize:{m_elementSize} Count:{m_count}}}";
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
