using System;
using System.Collections.Generic;

namespace SoSmooth.Scene
{
    /// <summary>
    /// Represents a scene that can contain entities.
    /// </summary>
    public class Scene
    {
        private List<Entity> m_rootEntities;
        private Camera m_active;
        
        public Scene()
        {
            m_rootEntities = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Can't add null entities to scene.");
            }
            // make sure the entity is removed from its previous scene
            if (entity.Scene != null)
            {
                entity.Scene.m_rootEntities.Remove(entity);
            }
            // add the entity to this scene root entities if it has no parent 
            if (entity.Transform == null || entity.Transform.Parent == null)
            {
                if (!m_rootEntities.Contains(entity))
                {
                    m_rootEntities.Add(entity);
                }
            }
            //entity.SetScene(this);
        }

        public void Render()
        {

        }
    }
}
