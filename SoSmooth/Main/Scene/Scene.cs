using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents a scene that can contain entities.
    /// </summary>
    public class Scene
    {
        private List<Entity> m_rootEntities;

        /// <summary>
        /// The camera the scene will render with.
        /// </summary>
        public Camera ActiveCamera { get; set; }
        
        public Scene()
        {
            m_rootEntities = new List<Entity>();
            
            for (int i = 0; i < 100; i++)
            {
                Entity cube = new Entity(this, "Cube");

                cube.Transform.LocalPosition = Random.GetVector3(25);
                cube.Transform.LocalRotation = Random.GetRotation();
                cube.Transform.LocalScale = Random.GetVector3(1);

                MeshRenderer camChildRenderer = new MeshRenderer(cube);
                camChildRenderer.Mesh = MeshBuilder.CreateCube();
                camChildRenderer.ShaderProgram = ShaderManager.SHADER_LIT;
            }

            Mesh mesh = MeshBuilder.CreateAxes();

            Entity entity1 = new Entity(this, "Axis1");
            MeshRenderer renderer = new MeshRenderer(entity1);
            renderer.Mesh = mesh;
            renderer.ShaderProgram = ShaderManager.SHADER_UNLIT;

            Entity entity2 = new Entity(this, "Axis2");
            entity2.Transform.Parent = entity1.Transform;
            entity2.Transform.LocalPosition = new Vector3(-1, 1, 1);
            MeshRenderer renderer2 = new MeshRenderer(entity2);
            renderer2.Mesh = mesh;
            renderer2.ShaderProgram = ShaderManager.SHADER_UNLIT;
        }
        
        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="resX">The framebuffer horizontal resolution.</param>
        /// <param name="resY">The framebuffer vertical resolution.</param>
        /// <returns>True if there is a camera that was able to render the scene.</returns>
        public bool Render(int resX, int resY)
        {
            if (ActiveCamera != null)
            {
                GL.ClearColor(ActiveCamera.ClearColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                ActiveCamera.SetResolution(resX, resY);
                
                // get all renderable components in the scene
                List<Renderable> renderables = new List<Renderable>();
                foreach (Entity entity in m_rootEntities)
                {
                    entity.GetComponentsInChildren(renderables);
                }

                // do culling on the components
                List<Renderable> toRender = new List<Renderable>();
                foreach (Renderable renderable in renderables)
                {
                    if (!renderable.IsCulled(ActiveCamera))
                    {
                        toRender.Add(renderable);
                    }
                }

                // sort the rendered components to minimize state changes
                toRender.Sort();

                // rendering of all components
                foreach (Renderable renderable in toRender)
                {
                    renderable.Render(ActiveCamera);
                }

                return true;
            }
            return false;
        }
        
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
