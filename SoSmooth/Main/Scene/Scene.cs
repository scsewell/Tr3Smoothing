using System.Collections.Generic;

namespace SoSmooth.Scene
{
    /// <summary>
    /// Represents a scene that can contain entities.
    /// </summary>
    public class Scene
    {
        private Camera m_active;

        private Entity m_root;
        
        public Scene()
        {
            m_root = new Entity("Root");
        }

        public void Render()
        {

        }
    }
}
