using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using SoSmooth.Rendering;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents a scene that can contain entities.
    /// </summary>
    public class Scene
    {
        private readonly List<Entity> m_rootEntities = new List<Entity>();

        /// <summary>
        /// The ambient lighting using in the scene.
        /// </summary>
        public Color4 AmbientLight = new Color4(0.05f, 0.05f, 0.05f, 1.0f);

        /// <summary>
        /// Finds all entities in this scene with a specific name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>A new list containing all entities with a matching name.</returns>
        public List<Entity> FindEntities(string name)
        {
            List<Entity> entities = new List<Entity>();
            foreach (Entity entity in m_rootEntities)
            {
                entity.Traverse((e) => {
                    if (e.Name == name)
                    {
                        entities.Add(e);
                    }
                });
            }
            return entities;
        }

        /// <summary>
        /// Finds all renderable components in the scene.
        /// </summary>
        /// <returns>A new list containing all renderable components.</returns>
        public List<Renderer> GetRenderables()
        {
            List<Renderer> renderables = new List<Renderer>();
            foreach (Entity entity in m_rootEntities)
            {
                entity.GetComponentsInChildren(renderables);
            }
            return renderables;
        }

        /// <summary>
        /// Gets the lighting data for the scene.
        /// </summary>
        /// <returns>The light data for the scene.</returns>
        public LightData GetLightData()
        {
            List<DirectionalLight> lights = new List<DirectionalLight>();
            foreach (Entity entity in m_rootEntities)
            {
                entity.GetComponentsInChildren(lights);
            }

            lights.Sort();

            return new LightData
            {
                AmbientColor = AmbientLight,

                Direction0 = (lights.Count > 0) ? lights[0].Transform.Forward : Vector3.Zero,
                Direction1 = (lights.Count > 1) ? lights[1].Transform.Forward : Vector3.Zero,
                Direction2 = (lights.Count > 2) ? lights[2].Transform.Forward : Vector3.Zero,

                DiffColor0 = (lights.Count > 0) ? lights[0].MainColor : Color4.Black,
                DiffColor1 = (lights.Count > 1) ? lights[1].MainColor : Color4.Black,
                DiffColor2 = (lights.Count > 2) ? lights[2].MainColor : Color4.Black,

                SpecColor0 = (lights.Count > 0) ? lights[0].SpecularColor : Color4.Black,
                SpecColor1 = (lights.Count > 1) ? lights[1].SpecularColor : Color4.Black,
                SpecColor2 = (lights.Count > 2) ? lights[2].SpecularColor : Color4.Black,
            };
        }

        /// <summary>
        /// Removes an entity from the root level of the scene heirarchy.
        /// </summary>
        /// <param name="entity">The entity that has changed scene.</param>
        internal void RemoveFromRoot(Entity entity)
        {
            m_rootEntities.Remove(entity);
        }

        /// <summary>
        /// Adds an entity to the root level of the scene heirarchy.
        /// </summary>
        /// <param name="entity">The entity that has changed scene.</param>
        internal void AddToRoot(Entity entity)
        {
            // root transforms must have no parent 
            if (entity.Transform == null || entity.Transform.Parent == null)
            {
                if (!m_rootEntities.Contains(entity))
                {
                    m_rootEntities.Add(entity);
                }
            }
        }
    }
}
