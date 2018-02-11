namespace SoSmooth.Rendering
{
    /// <summary>
    /// This interface must be implemented by any custom vertex data.
    /// </summary>
    public interface IVertexData
    {
        /// <summary>
        /// Returns the vertex's size./>
        /// </summary>
        /// <returns>The total size in bytes.</returns>
        int Size();

        /// <summary>
        /// Returns the vertex's <see cref="VertexAttributes"/>
        /// </summary>
        /// <returns>Array of <see cref="VertexAttribute"/></returns>
        VertexAttribute[] VertexAttributes();
    }
}
