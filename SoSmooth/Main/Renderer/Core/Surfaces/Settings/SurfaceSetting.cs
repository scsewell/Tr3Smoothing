
namespace SoSmooth.Rendering
{
    /// <summary>
    /// Base class for all surface settings.
    /// </summary>
    public abstract class SurfaceSetting
    {
        /// <summary>
        /// Sets the setting for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public abstract void Set(ShaderProgram program);
    }
}
