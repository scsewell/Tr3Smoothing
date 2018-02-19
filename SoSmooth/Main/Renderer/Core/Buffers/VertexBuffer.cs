using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class manages a vertex buffer object.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertex in the buffer.</typeparam>
    public sealed class VertexBuffer<TVertex> : DynamicBuffer<TVertex>, IVertexBuffer 
        where TVertex : struct, IVertexData
    {
        private static readonly VertexAttribute[] m_attributes = VertexDataHelper.MakeAttributeArray<TVertex>();

        /// <summary>
        /// The attributes of the vertices contained in this buffer.
        /// </summary>
        public VertexAttribute[] VertexAttributes => m_attributes;

        /// <summary>
        /// Initialises a new instance of <see cref="VertexBuffer{TVertexData}"/>.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public VertexBuffer(int capacity = 0) : base(BufferTarget.ArrayBuffer, capacity)
        {
        }
    }
}
