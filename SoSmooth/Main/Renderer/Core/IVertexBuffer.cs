using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Interface for vertex buffers with any type of vertex.
    /// </summary>
    public interface IVertexBuffer
    {
        /// <summary>
        /// The vertex attributes.
        /// </summary>
        VertexAttribute[] VertexAttributes { get; }

        /// <summary>
        /// The number of vertices in this VertexBuffer.
        /// </summary>
        ushort Count { get; }

        /// <summary>
        /// Binds the vertex buffer object.
        /// </summary>
        /// <param name="target">The target.</param>
        void Bind(BufferTarget target = BufferTarget.ArrayBuffer);

        /// <summary>
        /// Uploads the vertex buffer to the GPU.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="usageHint">The usage hint.</param>
        void BufferData(
            BufferTarget target = BufferTarget.ArrayBuffer, 
            BufferUsageHint usageHint = BufferUsageHint.DynamicDraw
        );

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
        void BufferNoData(
            int vertexCount, 
            BufferTarget target = BufferTarget.ArrayBuffer,
            BufferUsageHint usageHint = BufferUsageHint.StreamDraw, 
            bool setVertexCount = false
        );

        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        void Dispose();
    }
}
