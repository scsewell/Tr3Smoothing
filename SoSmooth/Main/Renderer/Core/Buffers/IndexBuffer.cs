using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class manages a index buffer object.
    /// </summary>
    /// <typeparam name="TIndex">Must be byte, ushort, or uint.</typeparam>
    public sealed class IndexBuffer<TIndex> : DynamicBuffer<TIndex>, IIndexBuffer where TIndex : struct
    {
        private readonly DrawElementsType m_elementType;
        private readonly long m_maxVerts;

        /// <summary>
        /// The value type used to index the vertices, limiting the indexable vertex count.
        /// </summary>
        public DrawElementsType ElementType
        {
            get
            {
                ValidateDispose();
                return m_elementType;
            }
        }

        /// <summary>
        /// The number of vertices that can be uniquely indexed using this index buffer.
        /// </summary>
        public long MaxVertices
        {
            get
            {
                ValidateDispose();
                return m_maxVerts;
            }
        }
        
        /// <summary>
        /// Initialises a new <see cref="IndexBuffer"/> instance.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public IndexBuffer(int capacity = 0) : base(BufferTarget.ElementArrayBuffer, capacity)
        {
            if (typeof(TIndex) == typeof(byte))
            {
                m_elementType = DrawElementsType.UnsignedByte;
                m_maxVerts = (long)byte.MaxValue + 1;
            }
            else if (typeof(TIndex) == typeof(ushort))
            {
                m_elementType = DrawElementsType.UnsignedShort;
                m_maxVerts = (long)ushort.MaxValue + 1;
            }
            else if (typeof(TIndex) == typeof(uint))
            {
                m_elementType = DrawElementsType.UnsignedInt;
                m_maxVerts = (long)uint.MaxValue + 1;
            }
            else
            {
                new Exception("Unsupported index buffer type: " + GetType().Name);
            }
        }
    }
}
