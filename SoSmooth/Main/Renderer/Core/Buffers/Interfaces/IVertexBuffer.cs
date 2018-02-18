namespace SoSmooth.Rendering
{
    /// <summary>
    /// Interface for vertex buffers with any type of vertex.
    /// </summary>
    public interface IVertexBuffer : IBuffer
    {
        /// <summary>
        /// The attributes of the vertices contained in this buffer.
        /// </summary>
        VertexAttribute[] VertexAttributes { get; }
    }
}
