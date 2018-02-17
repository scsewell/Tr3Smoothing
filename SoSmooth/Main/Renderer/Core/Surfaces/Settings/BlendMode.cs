namespace SoSmooth.Rendering
{
    /// <summary>
    /// Blend modes that determine finished fragments are combined with the render target.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// The fragment will overwrite the render target's existing color.
        /// </summary>
        None,
        /// <summary>
        /// Alpha blending using the fragment's alpha.
        /// </summary>
        Alpha,
        /// <summary>
        /// Alpha blending using the fragment color with premultiplied alpha.
        /// </summary>
        PremultipliedAlpha,
        /// <summary>
        /// Add the fragment's color to the render target's current color.
        /// </summary>
        Add,
        /// <summary>
        /// Subtract the fragment's color from the render target's current color.
        /// </summary>
        Subtract,
        /// <summary>
        /// Multiply the fragment's color against the render target's current color.
        /// </summary>
        Multiply,
        /// <summary>
        /// Use the component-wise min of the fragment's color and render target's current color.
        /// </summary>
        Min,
        /// <summary>
        /// Use the component-wise max of the fragment's color and render target's current color.
        /// </summary>
        Max,
    }
}
