using System;
using System.Collections.Generic;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents an object that can exist in the scene.
    /// </summary>
    public class Entity : Disposable
    {
        private string m_name;
        private List<Component> m_components;
        private Transform m_transform;
        private Scene m_scene;

        /// <summary>
        /// The name of this entity.
        /// </summary>
        public string Name
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_name;
            }
            set
            {
                if (m_name != value)
                {
                    SetName(value);
                }
            }
        }

        /// <summary>
        /// The scene this entity belongs to.
        /// </summary>
        public Scene Scene
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_scene;
            }
            set
            {
                if (m_scene != value)
                {
                    SetScene(value);
                }
            }
        }
        
        /// <summary>
        /// The transform attached to this entity.
        /// </summary>
        public Transform Transform
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_transform;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scene">The scene to add the entity to.</param>
        /// <param name="name">The name of the entity. Must be non-null and non-whitespace only.</param>
        public Entity(Scene scene, string name = "NewEntity")
        {
            m_components = new List<Component>();
            m_transform = new Transform(this, OnChangeParent);

            SetScene(scene);
            SetName(name);
        }

        /// <summary>
        /// Changes the scene that this entity belongs to. Will also set
        /// the scene for all children. Clears the parent, putting this
        /// entity in the top level of the new scene's heirarchy.
        /// </summary>
        /// <param name="scene">The new scene. Must not be null.</param>
        private void SetScene(Scene scene)
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            if (scene == null)
            {
                throw new ArgumentException("Entity scene must not be null.");
            }
            Transform.SetParent(null);
            scene.AddToRoot(this);
            Traverse((entity) => entity.m_scene = scene);
        }

        /// <summary>
        /// Checks if a name is valid to use and sets it.
        /// </summary>
        /// <param name="name">The name of the entity. Must be non-null and non-whitespace only.</param>
        private void SetName(string name)
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            if (name == null)
            {
                throw new ArgumentException("Entity name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Entity name must contain non-whitespace characters.");
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
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            if (component == null)
            {
                throw new ArgumentException("Can't attach a null component to entity: " + this);
            }
            if (component.Entity != null)
            {
                throw new ArgumentException("Component is already attached to an entity: " + component.Entity);
            }
            if (typeof(T) == typeof(Transform) && m_transform != null)
            {
                throw new ArgumentException("Entity already has transform component: " + this);
            }
            
            m_components.Add(component);
            return component;
        }

        /// <summary>
        /// Finds the first instance of a given type of component on this entity.
        /// </summary>
        /// <typeparam name="T">The type of component to find instances of.</typeparam>
        /// <returns> The first instance or null if not found.</returns>
        public T GetComponent<T>() where T : Component
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            foreach (Component c in m_components)
            {
                if (typeof(T).IsAssignableFrom(c.GetType()))
                {
                    return (T)c;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the instances of a given type of component on this entity.
        /// </summary>
        /// <typeparam name="T">The type of component to find instances of.</typeparam>
        /// <returns> All instances or empty list if nothing found.</returns>
        public List<T> GetComponents<T>() where T : Component
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            List<T> components = new List<T>();
            GetComponents(components);
            return components;
        }

        /// <summary>
        /// Finds the instances of a given type of component on this entity.
        /// </summary>
        /// <typeparam name="T">The type of component to find instances of.</typeparam>
        /// <param name="result">The list discovered components are appended to.</param>
        public void GetComponents<T>(List<T> result) where T : Component
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            foreach (Component c in m_components)
            {
                if (typeof(T).IsAssignableFrom(c.GetType()))
                {
                    result.Add((T)c);
                }
            }
        }

        /// <summary>
        /// Finds the instances of a given type of component on this entity
        /// or any of its children.
        /// </summary>
        /// <typeparam name="T">The type of component to find instances of.</typeparam>
        /// <returns> All instances or empty list if nothing found.</returns>
        public List<T> GetComponentsInChildren<T>() where T : Component
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            List<T> components = new List<T>();
            GetComponentsInChildren(components);
            return components;
        }

        /// <summary>
        /// Finds the instances of a given type of component on this entity
        /// or any of its children.
        /// </summary>
        /// <typeparam name="T">The type of component to find instances of.</typeparam>
        /// <param name="result">The list discovered components are appended to.</param>
        public void GetComponentsInChildren<T>(List<T> result) where T : Component
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            Traverse((entity) => entity.GetComponents(result));
        }

        /// <summary>
        /// Recursively calls some function on this entity and all of
        /// its decendants in the heirarchy.
        /// </summary>
        /// <param name="action">A function to perform on each entity.</param>
        public void Traverse(Action<Entity> action)
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            action(this);
            foreach (Transform child in Transform.Children)
            {
                child.Entity.Traverse(action);
            }
        }

        /// <summary>
        /// Handles a change in parent.
        /// </summary>
        /// <param name="oldParent">The old parent transform.</param>
        /// <param name="newParent">The new parent transform.</param>
        private void OnChangeParent(Transform oldParent, Transform newParent)
        {
            if (newParent != null)
            {
                if (oldParent == null)
                {
                    m_scene.RemoveFromRoot(this);
                }
                // ensure that this entity and all children inherit the scene of the new parent
                Traverse((entity) => entity.m_scene = newParent.Entity.m_scene);
            }
            else // if the parent was cleared add the entity to the root of the scene
            {
                m_scene.AddToRoot(this);
            }
        }

        /// <summary>
        /// Gets a nicely formatted string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Removes this entity and its children.
        /// Cleans up any resources held by these entities.
        /// </summary>
        protected override void OnDispose(bool disposing)
        {
            // remove from scene / parent
            m_transform.Parent = null;
            m_scene.RemoveFromRoot(this);

            // recursively disposes all children
            List<Transform> children = new List<Transform>(m_transform.Children);
            foreach (Transform child in children)
            {
                if (!child.Disposed)
                {
                    child.Entity.Dispose();
                }
            }

            // dispose all components
            foreach (Component component in m_components)
            {
                component.Dispose();
            }

            m_name = null;
            m_components = null;
            m_transform = null;
            m_scene = null;
        }
    }
}
