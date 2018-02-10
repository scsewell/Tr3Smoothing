namespace SoSmooth.Scene
{
    /// <summary>
    /// Represents a component that can be added to an entity.
    /// </summary>
    public abstract class Component
    {
        private readonly Entity m_entity;

        /// <summary>
        /// The entity this component is attached to.
        /// </summary>
        public Entity Entity
        {
            get { return m_entity; }
        }

        /// <summary>
        /// The transform attached to this component's entity.
        /// </summary>
        public Transform Transform
        {
            get { return m_entity.Transform; }
        }

        /// <summary>
        /// Creates a new component and attaches it to an entity.
        /// </summary>
        /// <param name="entity">The entity the new component will belong to.</param>
        public Component(Entity entity)
        {
            entity.AttachComponent(this);
            m_entity = entity;
        }

        /// <summary>
        /// Gets a nicely formatted string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return "(" + GetType().Name + ") " + m_entity.Name;
        }
    }
}
