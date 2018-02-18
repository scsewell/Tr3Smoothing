using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class manages a vertex buffer object.
    /// </summary>
    /// <typeparam name="TVertexData">The type of vertex in the buffer.</typeparam>
    public sealed class VertexBuffer<TVertexData> : DynamicBuffer<TVertexData>, IVertexBuffer 
        where TVertexData : struct, IVertexData
    {
        /// <summary>
        /// A mapping that stores the vertex attributes of any vertex types that have been used in a buffer.
        /// </summary>
        private static Dictionary<Type, VertexAttribute[]> m_typeSizes = new Dictionary<Type, VertexAttribute[]>();

        private readonly VertexAttribute[] m_vertexAttributes;

        /// <summary>
        /// The attributes of the vertices contained in this buffer.
        /// </summary>
        public VertexAttribute[] VertexAttributes => m_vertexAttributes;

        /// <summary>
        /// Initialises a new instance of <see cref="VertexBuffer{TVertexData}"/>.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public VertexBuffer(int capacity = 0) : base(BufferTarget.ArrayBuffer, capacity)
        {
            Type dataType = typeof(DataType);

            if (!m_typeSizes.TryGetValue(dataType, out m_vertexAttributes))
            {
                m_vertexAttributes = VertexDataHelper.MakeAttributeArray<TVertexData>();
                m_typeSizes.Add(dataType, m_vertexAttributes);
            }
        }
    }
}
