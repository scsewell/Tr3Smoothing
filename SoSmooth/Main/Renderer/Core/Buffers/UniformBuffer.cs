using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Manages a uniform buffer object.
    /// </summary>
    public class UniformBuffer<TData> : Buffer<TData> where TData : struct, IEquatable<TData>
    {
        /// <summary>
        /// The value of the uniform in the buffer.
        /// </summary>
        public TData Value
        {
            get { return m_buffer[0]; }
            set
            {
                if (!m_buffer[0].Equals(value))
                {
                    m_buffer[0] = value;
                    m_dirty = true;
                }
            }
        }

        /// <summary>
        /// Initialises a new <see cref="UniformBuffer{Data}"/> instance.
        /// </summary>
        public UniformBuffer() : base(BufferTarget.UniformBuffer, 1)
        {
            m_count = 1;
        }
    }
}
