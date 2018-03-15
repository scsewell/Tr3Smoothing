using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Manages a uniform buffer object.
    /// </summary>
    public class UniformBuffer<TData> : Buffer<TData>, IUniformBuffer where TData : struct, IEquatable<TData>
    {
        private readonly int m_bindingPoint;

        /// <summary>
        /// The value of the uniform in the buffer.
        /// </summary>
        public TData Value
        {
            get
            {
                ValidateDispose();
                return m_buffer[0];
            }
            set
            {
                ValidateDispose();
                if (!m_buffer[0].Equals(value))
                {
                    m_buffer[0] = value;
                    m_dirty = true;
                }
            }
        }

        /// <summary>
        /// The binding point index for this uniform block.
        /// </summary>
        public int BindingPoint
        {
            get
            {
                ValidateDispose();
                return m_bindingPoint;
            }
        }
        
        /// <summary>
        /// Initialises a new <see cref="UniformBuffer{Data}"/> instance.
        /// </summary>
        public UniformBuffer() : base(BufferTarget.UniformBuffer, 1)
        {
            m_bindingPoint = BlockManager.GetBindingPoint(typeof(TData).Name);
            
            m_count = 1;
            
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, m_bindingPoint, this);
        }
    }
}
