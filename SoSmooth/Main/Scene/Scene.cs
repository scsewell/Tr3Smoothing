using System;
using System.Collections.Generic;
using OpenTK;
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
        private Camera m_activeCamera;
        
        public Scene()
        {
            m_rootEntities = new List<Entity>();
            
            Entity cam = new Entity(this, "Camera");
            m_camera = new Camera(cam);
            m_camera.Transform.LocalPosition = new Vector3(0, -5, 0);
            m_camera.Transform.LocalRotation = Quaternion.FromEulerAngles(0, 0, MathHelper.PiOver2);

            m_camChild = new Entity(this, "CamChildCube");
            m_camChild.Transform.LocalPosition = new Vector3(0, 0, -8);
            m_camChild.Transform.SetParent(cam.Transform, false);
            MeshRenderer camChildRenderer = new MeshRenderer(m_camChild);
            camChildRenderer.SetMesh(Mesh<VertexNC>.CreateCube());
            camChildRenderer.SetProgram(ShaderManager.Instance.GetProgram("unlit"));

            m_entity1 = new Entity(this, "Cube");
            MeshRenderer renderer = new MeshRenderer(m_entity1);
            renderer.SetMesh(Mesh<VertexNC>.CreateDirectionThing());
            renderer.SetProgram(ShaderManager.Instance.GetProgram("unlit"));

            m_entity2 = new Entity(this, "Cube2");
            m_entity2.Transform.Parent = m_entity1.Transform;
            m_entity2.Transform.LocalPosition = new Vector3(-1, 1, 1);
            MeshRenderer renderer2 = new MeshRenderer(m_entity2);
            renderer2.SetMesh(Mesh<VertexNC>.CreateDirectionThing());
            renderer2.SetProgram(ShaderManager.Instance.GetProgram("unlit"));

            Logger.Debug(m_entity1.GetComponentsInChildren<Transform>().Count);

            SetActiveCamera(m_camera);
        }
        
        private Camera m_camera;
        private Entity m_camChild;
        private Entity m_entity1;
        private Entity m_entity2;
        
        /// <summary>
        /// Sets which camera the scene will render with.
        /// </summary>
        /// <param name="camera">The camera to make active.</param>
        public void SetActiveCamera(Camera camera)
        {
            m_activeCamera = camera;
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="resX">The framebuffer horizontal resolution.</param>
        /// <param name="resY">The framebuffer vertical resolution.</param>
        public void Render(int resX, int resY)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(m_activeCamera != null ? m_activeCamera.ClearColor : Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (m_activeCamera != null)
            {
                m_activeCamera.SetResolution(resX, resY);

                //m_camera.Transform.LocalPosition = new Vector3(0, -5 + (float)Math.Sin(Time.time), 0);
                //m_camera.Transform.LocalRotation = Quaternion.FromEulerAngles(0, 0, MathHelper.PiOver2 * (float)Math.Sin(Time.time));

                //m_entity1.Transform.LocalScale = new Vector3((float)Math.Sin(Time.time) + 2, 1, 1);
                m_entity1.Transform.LocalPosition = new Vector3((float)Math.Sin(Time.time), 0, 0);
                m_entity1.Transform.LocalRotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 1), Time.time);

                m_entity2.Transform.LocalScale = new Vector3((float)Math.Sin(Time.time) + 2, 1, 1);
                m_entity2.Transform.LocalRotation = Quaternion.FromAxisAngle(new Vector3(1, 1, 1), Time.time).Inverted();

                m_camera.Transform.SetParent((Time.time % 3 > 1.5f) ? m_entity1.Transform : null, true);

                m_entity1.GetComponent<MeshRenderer>().Render(m_camera);
                m_entity2.GetComponent<MeshRenderer>().Render(m_camera);
                m_camChild.GetComponent<MeshRenderer>().Render(m_camera);

                Logger.Debug(m_camChild.Transform.Root);
            }
        }

        /// <summary>
        /// Handles moving an entity from one scene to another.
        /// </summary>
        /// <param name="entity">The entity that has changed scene.</param>
        /// <param name="oldScene">The scene the entity was in.</param>
        /// <param name="newScene">The scene the entity was added to.</param>
        internal static void OnSceneChange(Entity entity, Scene oldScene, Scene newScene)
        {
            // make sure the entity is removed from its previous scene
            if (oldScene != null)
            {
                oldScene.m_rootEntities.Remove(entity);
            }
            // add the entity to this scene root entities if it has no parent 
            if (entity.Transform == null || entity.Transform.Parent == null)
            {
                if (!newScene.m_rootEntities.Contains(entity))
                {
                    newScene.m_rootEntities.Add(entity);
                }
            }
        }
    }
}
