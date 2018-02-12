using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;
using SoSmooth.Rendering.Meshes;

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
            
            m_camChild = new Entity(this, "CamChildCube");

            m_camChild.Transform.LocalScale = new Vector3(1.5f, 1.5f, 1.5f);
            m_camChild.Transform.LocalPosition = new Vector3(0, 3, 0);
            MeshRenderer camChildRenderer = new MeshRenderer(m_camChild);
            camChildRenderer.Mesh = MeshBuilder.CreateCube();
            camChildRenderer.ShaderProgram = ShaderManager.SHADER_UNLIT;

            Mesh mesh = MeshBuilder.CreateDirectionThing();

            m_entity1 = new Entity(this, "Cube");
            MeshRenderer renderer = new MeshRenderer(m_entity1);
            renderer.Mesh = mesh;
            renderer.ShaderProgram = ShaderManager.SHADER_UNLIT;

            m_entity2 = new Entity(this, "Cube2");
            m_entity2.Transform.Parent = m_entity1.Transform;
            m_entity2.Transform.LocalPosition = new Vector3(-1, 1, 1);
            MeshRenderer renderer2 = new MeshRenderer(m_entity2);
            renderer2.Mesh = mesh;
            renderer2.ShaderProgram = ShaderManager.SHADER_UNLIT;
        }
        
        private Entity m_camChild;
        private Entity m_entity1;
        private Entity m_entity2;
        
        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="resX">The framebuffer horizontal resolution.</param>
        /// <param name="resY">The framebuffer vertical resolution.</param>
        public void Render(int resX, int resY)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);

            GL.ClearColor(ActiveCamera != null ? ActiveCamera.ClearColor : Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            if (ActiveCamera != null)
            {
                ActiveCamera.SetResolution(resX, resY);
                /*
                m_entity1.Transform.LocalPosition = new Vector3((float)Math.Sin(Time.time), 0, 0);
                m_entity1.Transform.LocalRotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 1), Time.time);

                m_entity2.Transform.LocalScale = new Vector3((float)Math.Sin(Time.time) + 2, 1, 1);
                m_entity2.Transform.LocalRotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 1), Time.time).Inverted();

                m_camChild.Transform.LocalRotation = Quaternion.FromAxisAngle(new Vector3(1, -1, 1), Time.time);

                m_camChild.Transform.SetParent((Time.time % 3 > 1.5f) ? m_entity1.Transform : null, true);
                m_entity2.GetComponent<MeshRenderer>().Mesh.UseColors = (Time.time % 6 > 3f);
                */
                // get all renderable components in the scene
                List<Renderable> renderables = new List<Renderable>();
                foreach (Entity entity in m_rootEntities)
                {
                    entity.GetComponentsInChildren(renderables);
                }

                // handle the rendering of all components
                foreach (Renderable renderable in renderables)
                {
                    renderable.Render(ActiveCamera);
                }
            }

            // report the latest error when rendering the scene, if applicable
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Logger.Error("OpenGL Error: " + error);
            }
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
