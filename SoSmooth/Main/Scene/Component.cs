using System;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents a component that can be added to an entity.
    /// </summary>
    public abstract class Component : Disposable
    {
        private Entity m_entity;

        /// <summary>
        /// The entity this component is attached to.
        /// </summary>
        public Entity Entity
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_entity;
            }
        }

        /// <summary>
        /// The transform attached to this component's entity.
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_entity.Transform;
            }
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

        /// <summary>
        /// Disposes this component and frees held resources
        /// </summary>
        /// <param name="entity">True if managed resources should be cleaned up.</param>
        protected override void OnDispose(bool disposing)
        {
            m_entity = null;
        }
    }
}
