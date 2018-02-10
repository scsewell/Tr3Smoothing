using System;
using System.Collections.Generic;

namespace SoSmooth.Scene
{
    /// <summary>
    /// Represents an object that can exist in the scene.
    /// </summary>
    public class Entity
    {
        private string m_name;

        /// <summary>
        /// The name of this entity.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { SetName(value); }
        }

        private List<Component> m_components;
        private Transform m_transform;

        /// <summary>
        /// The transform attached to this entity.
        /// </summary>
        public Transform Transform
        {
            get { return m_transform; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the entity. Must be non-null and non-whitespace only.</param>
        public Entity(string name = "NewEntity")
        {
            SetName(name);

            m_components = new List<Component>();

            m_transform = new Transform(this);
        }

        /// <summary>
        /// Checks if a name is valid to use and sets it.
        /// </summary>
        /// <param name="name">The new name.</param>
        private void SetName(string name)
        {
            if (name == null)
            {
                throw new ArgumentException("Must not be null.", "name");
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Must contain non-whitespace characters.", "name");
            }
            m_name = name;
        }

        /// <summary>
        /// Attaches a component to an entity.
        /// </summary>
        /// <param name="component">The new component to add to this entity.</param>
        /// <returns> The component intstance.</returns>
        public T AttachComponent<T>(T component) where T : Component
        {
            if (component.Entity != null)
            {
                throw new ArgumentException("Component is already attached to an entity: " + component.Entity);
            }
            if (typeof(T) == typeof(Transform) && m_transform != null)
            {
                throw new ArgumentException("Entity already has transform component: " + Name);
            }
            
            m_components.Add(component);
            return component;
        }

        /// <summary>
        /// Finds the first instance of a given type of component on this entity.
        /// </summary>
        /// <returns> The first instance or null if not found.</returns>
        public T GetComponent<T>() where T : Component
        {
            foreach (Component c in m_components)
            {
                if (typeof(T) == c.GetType())
                {
                    return (T)c;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the instances of a given type of component on this entity.
        /// </summary>
        /// <returns> All instances or empty list if not found.</returns>
        public List<T> GetComponents<T>() where T : Component
        {
            List<T> components = new List<T>();
            foreach (Component c in m_components)
            {
                if (typeof(T) == c.GetType())
                {
                    components.Add((T)c);
                }
            }
            return components;
        }
    }
}
