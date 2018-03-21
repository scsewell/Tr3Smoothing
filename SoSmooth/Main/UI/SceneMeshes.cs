using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Gdk;
using Gtk;
using SoSmooth.Meshes;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// Manages the visuals of and interactions with meshes in the scene.
    /// </summary>
    public class SceneMeshes
    {
        private readonly Dictionary<MeshInfo, Entity> m_meshToEntity = new Dictionary<MeshInfo, Entity>();
        private readonly Dictionary<Entity, MeshInfo> m_entityToMesh = new Dictionary<Entity, MeshInfo>();
        private readonly Entity m_meshesRoot;
        private readonly SceneWindow m_window;

        public string frontFaceMode = "Solid";
        public string backFaceMode = "Solid";
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">The scene window to add meshes to.</param>
        public SceneMeshes(SceneWindow window)
        {
            m_window = window;
            m_window.Update += OnUpdate;

            m_meshesRoot = new Entity(window.Scene, "MeshesRoot");
            
            MeshManager.Instance.MeshesAdded += OnMeshesAdded;
            MeshManager.Instance.MeshRemoved += OnMeshRemoved;
            MeshManager.Instance.SelectionChanged += OnSelectionChanged;
            MeshManager.Instance.VisibilityChanged += OnVisibilityChanged;
        }

        /// <summary>
        /// Called when a mesh is added to the scene.
        /// </summary>
        private void OnMeshesAdded(IEnumerable<MeshInfo> meshes)
        {
            foreach (MeshInfo mesh in meshes)
            {
                Entity entity = new Entity(m_meshesRoot, mesh.Mesh.Name);
                m_meshToEntity[mesh] = entity;
                m_entityToMesh[entity] = mesh;
                
                // have one renderer for front and back faces separately
                MeshRenderer frontRenderer = new MeshRenderer(entity, mesh.Mesh);
                frontRenderer.CullMode = Rendering.CullMode.Back;
                frontRenderer.Color = mesh.Color;

                MeshRenderer backRenderer = new MeshRenderer(entity, mesh.Mesh);
                backRenderer.CullMode = Rendering.CullMode.Front;
                backRenderer.Color = mesh.Color;

                BoundsRenderer boundsRenderer = new BoundsRenderer(entity, mesh.Mesh);
                boundsRenderer.Enabled = false;
                boundsRenderer.Color = mesh.Color;
            }
            // zoom to the new meshes if nothing is selected
            if (MeshManager.Instance.SelectedMeshes.Count == 0)
            {
                m_window.Camera.EaseToMeshes(meshes);
            }
        }

        /// <summary>
        /// Called when a mesh is removed from the scene.
        /// </summary>
        private void OnMeshRemoved(MeshInfo mesh)
        {
            Entity entity;
            if (m_meshToEntity.TryGetValue(mesh, out entity))
            {
                m_meshToEntity.Remove(mesh);
                m_entityToMesh.Remove(entity);
                entity.Dispose();
            }
        }

        /// <summary>
        /// Called when the selected meshes have changed.
        /// Enables drawing bounding boxes around selected meshes.
        /// </summary>
        private void OnSelectionChanged(IEnumerable<MeshInfo> selected)
        {
            foreach (BoundsRenderer renderer in m_meshesRoot.GetComponentsInChildren<BoundsRenderer>())
            {
                renderer.Enabled = false;
            }
            foreach (MeshInfo mesh in selected)
            {
                m_meshToEntity[mesh].GetComponent<BoundsRenderer>().Enabled = true;
            }
        }

        /// <summary>
        /// Called when the visibility for a mesh has been changed.
        /// </summary>
        private void OnVisibilityChanged(MeshInfo mesh, bool newVisibility)
        {
            Entity entity;
            if (m_meshToEntity.TryGetValue(mesh, out entity))
            {
                foreach (MeshRenderer renderer in entity.GetComponents<MeshRenderer>())
                {
                    renderer.Enabled = newVisibility;
                }
            }
        }

        /// <summary>
        /// Gets the entity for a mesh in this scene.
        /// </summary>
        /// <param name="mesh">A mesh that is in the scene.</param>
        /// <returns>The entity corresponding to the mesh.</returns>
        public Entity GetEntity(MeshInfo mesh)
        {
            return m_meshToEntity[mesh];
        }

        /// <summary>
        /// Called prior to rendering.
        /// </summary>
        private void OnUpdate()
        {
            // apply the front and backface render modes to the renderers.
            foreach (Transform child in m_meshesRoot.Transform.Children)
            {
                MeshInfo mesh = m_entityToMesh[child.Entity];
                foreach (MeshRenderer renderer in child.Entity.GetComponents<MeshRenderer>())
                {
                    if (renderer.CullMode == Rendering.CullMode.Back)
                    {
                        switch (frontFaceMode)
                        {
                            case "Solid":
                                renderer.FrontFaceMode = PolygonMode.Fill;
                                renderer.ShaderProgram = ShaderManager.SHADER_LIT;
                                renderer.Enabled = mesh.IsVisible;
                                break;
                            case "Wireframe":
                                renderer.FrontFaceMode = PolygonMode.Line;
                                renderer.ShaderProgram = ShaderManager.SHADER_UNLIT;
                                renderer.Enabled = mesh.IsVisible;
                                break;
                            case "Hidden":
                                renderer.Enabled = false;
                                break;
                        }
                    }
                    else
                    {
                        switch (backFaceMode)
                        {
                            case "Solid":
                                renderer.BackFaceMode = PolygonMode.Fill;
                                renderer.ShaderProgram = ShaderManager.SHADER_LIT;
                                renderer.Enabled = mesh.IsVisible;
                                break;
                            case "Wireframe":
                                renderer.BackFaceMode = PolygonMode.Line;
                                renderer.ShaderProgram = ShaderManager.SHADER_UNLIT;
                                renderer.Enabled = mesh.IsVisible;
                                break;
                            case "Hidden":
                                renderer.Enabled = false;
                                break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when a mouse button was pressed.
        /// </summary>
        public void OnButtonPress(ButtonPressEventArgs args)
        {
            Vector2 mousePos = new Vector2((float)args.Event.X, (float)args.Event.Y);

            ModifierType modifierMask = Accelerator.DefaultModMask;
            bool shift = (args.Event.State & modifierMask) == ModifierType.ShiftMask;

            if (args.Event.Button == 1) // left click
            {
                // find the nearest mesh under the cursor and select it
                Camera cam = m_window.Camera.Camera;
                Vector3 rayOrigin = cam.Transform.Position;
                Vector3 rayDir = cam.ScreenPointToWorldDirection(mousePos);

                MeshInfo clicked = null;
                float minDistance = float.MaxValue;

                foreach (MeshInfo info in MeshManager.Instance.VisibleMeshes)
                {
                    Entity entity = m_meshToEntity[info];
                    Mesh mesh = info.Mesh;

                    // we do the calculations in local space to avoid needing to transform
                    // the mesh vertices to world space.
                    Matrix4 toLocal = entity.Transform.WorldToLocalMatrix;

                    Vector3 orig = Vector3.TransformPosition(rayOrigin, toLocal);
                    Vector3 dir = Vector3.TransformVector(rayDir, toLocal);

                    // first check if the mesh's bounding box is under the cursor
                    if (mesh.Bounds.Raycast(orig, dir))
                    {
                        Matrix4 toWorld = entity.Transform.LocalToWorldMatix;

                        Vector3[] verts = mesh.Vertices;
                        Triangle[] tris = mesh.Triangles;

                        for (int i = 0; i < tris.Length; i++)
                        {
                            Vector3 localIntersect;
                            Triangle tri = tris[i];

                            if (Utils.RaycastTriangle(
                                verts[tri.index0],
                                verts[tri.index1],
                                verts[tri.index2],
                                orig, dir, out localIntersect))
                            {
                                // transform the intersect back into world space
                                Vector3 intersect = Vector3.TransformPosition(localIntersect, toWorld);
                                float dist = (intersect - rayOrigin).Length;

                                // select the mesh if close but not before the camera newr clip plane
                                if (dist < minDistance && dist > cam.NearClip)
                                {
                                    minDistance = dist;
                                    clicked = info;
                                }
                            }
                        }
                    }
                }
                // add to the current selection if shift is pressed
                MeshManager.Instance.SetActiveMesh(clicked, !shift);
            }
        }
    }
}
