namespace SoSmooth.Scenes
{
    /// <summary>
    /// A component that can be drawn in the scene.
    /// </summary>
    public abstract class Renderable : Component
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Renderable(Entity entity) : base(entity)
        { }

        /// <summary>
        /// Renders this component.
        /// </summary>
        /// <param name="camera">The camera used for rendering.</param>
        public abstract void Render(Camera camera);
    }
}
