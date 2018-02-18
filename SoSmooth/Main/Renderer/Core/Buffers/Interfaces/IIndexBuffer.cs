using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    public interface IIndexBuffer : IBuffer
    {
        /// <summary>
        /// The value type used to index the vertices, limiting the indexable vertex count.
        /// </summary>
        DrawElementsType ElementType { get; }

        /// <summary>
        /// The number of vertices that can be uniquely indexed using this index buffer.
        /// </summary>
        long MaxVertices { get; }
    }
}
