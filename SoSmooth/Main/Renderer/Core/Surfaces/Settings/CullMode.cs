namespace SoSmooth.Rendering
{
    /// <summary>
    /// Specifies which faces are culled when rendering.
    /// </summary>
    public enum CullMode
    {
        /// <summary>
        /// All faces are rendered.
        /// </summary>
        Off,
        /// <summary>
        /// Only front faces are rendered. 
        /// </summary>
        Back,
        /// <summary>
        /// Only back faces are rendered.
        /// </summary>
        Front,
    }
}
