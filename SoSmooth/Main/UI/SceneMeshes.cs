using System;
using System.Collections.Generic;
using System.Linq;
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

        public enum RenderMode
        {
            Solid,
            Wireframe,
            Hidden,
        }

        private RenderMode m_frontFaceMode = RenderMode.Solid;
        private RenderMode m_backFaceMode = RenderMode.Solid;

        /// <summary>
        /// Sets the render mode of the front faces of all meshes.
        /// </summary>
        public RenderMode FrontFaceMode
        {
            get { return m_frontFaceMode; }
            set
            {
                if (m_frontFaceMode != value)
                {
                    m_frontFaceMode = value;
                    SetRenderModeAll();
                }
            }
        }

        /// <summary>
        /// Sets the render mode of the back faces of all meshes.
        /// </summary>
        public RenderMode BackFaceMode
        {
            get { return m_backFaceMode; }
            set
            {
                if (m_backFaceMode != value)
                {
                    m_backFaceMode = value;
                    SetRenderModeAll();
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">The scene window to add meshes to.</param>
        public SceneMeshes(SceneWindow window)
        {
            m_window = window;
            m_meshesRoot = new Entity(window.Scene, "MeshesRoot");
            
            MeshManager.Instance.MeshesAdded += OnMeshesAdded;
            MeshManager.Instance.MeshRemoved += OnMeshRemoved;
        }

        /// <summary>
        /// Called when a mesh is added to the scene.
        /// </summary>
        private void OnMeshesAdded(IEnumerable<MeshInfo> meshes)
        {
            // add the meshes to the scene
            foreach (MeshInfo mesh in meshes)
            {
                mesh.SelectionChanged += OnSelectionChanged;
                mesh.VisibilityChanged += OnVisibilityChanged;

                Entity entity = new Entity(m_meshesRoot, mesh.Mesh.Name);
                m_meshToEntity[mesh] = entity;
                m_entityToMesh[entity] = mesh;
                
                // have one renderer for front and back faces separately
                MeshRenderer frontRenderer = new MeshRenderer(entity, mesh.Mesh);
                frontRenderer.CullMode = Rendering.CullMode.Back;
                frontRenderer.Color = mesh.Color;
                SetRenderMode(frontRenderer, m_frontFaceMode);

                MeshRenderer backRenderer = new MeshRenderer(entity, mesh.Mesh);
                backRenderer.CullMode = Rendering.CullMode.Front;
                backRenderer.Color = mesh.Color;
                SetRenderMode(backRenderer, m_backFaceMode);

                BoundsRenderer boundsRenderer = new BoundsRenderer(entity, mesh.Mesh);
                boundsRenderer.Enabled = mesh.IsSelected;
                boundsRenderer.Color = mesh.Color;
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
                mesh.SelectionChanged -= OnSelectionChanged;
                mesh.VisibilityChanged -= OnVisibilityChanged;

                m_meshToEntity.Remove(mesh);
                m_entityToMesh.Remove(entity);
                entity.Dispose();
            }
        }

        /// <summary>
        /// Called when the selected meshes have changed.
        /// Enables drawing bounding boxes around selected meshes.
        /// </summary>
        private void OnSelectionChanged(MeshInfo mesh)
        {
            Entity entity;
            if (m_meshToEntity.TryGetValue(mesh, out entity))
            {
                entity.GetComponent<BoundsRenderer>().Enabled = mesh.IsSelected;
            }
        }

        /// <summary>
        /// Called when the visibility for a mesh has been changed.
        /// </summary>
        private void OnVisibilityChanged(MeshInfo mesh)
        {
            Entity entity;
            if (m_meshToEntity.TryGetValue(mesh, out entity))
            {
                foreach (MeshRenderer renderer in entity.GetComponents<MeshRenderer>())
                {
                    renderer.Enabled = mesh.IsVisible;
                }
            }
        }

        /// <summary>
        /// Moves the camera to view the selected meshes.
        /// </summary>
        public void FocusSelected()
        {
            m_window.Camera.EaseToBounds(GetBounds(MeshManager.Instance.SelectedMeshes));
        }

        /// <summary>
        /// Gets the bounding box of meshes in the scene.
        /// </summary>
        public Bounds GetBounds(IEnumerable<MeshInfo> meshes)
        {
            List<Vector3> boundCorners = new List<Vector3>();

            // get the bounding box for all of the meshes to view in the scene
            foreach (MeshInfo mesh in meshes)
            {
                Entity entity = m_meshToEntity[mesh];
                Bounds b = mesh.Mesh.Bounds.Transformed(entity.Transform.LocalToWorldMatix);
                boundCorners.AddRange(b.Corners);
            }

            // get a bounding box around all the individual bounds for each mesh
            if (boundCorners.Count > 0)
            {
                return Bounds.FromPoints(boundCorners.AsReadOnly());
            }
            return default(Bounds);
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
                MeshInfo underCursor = GetMeshAtScreenPos(mousePos);

                ModifyMeshInfosOperation op = new ModifyMeshInfosOperation();

                // clear current selection unless shift is pressed
                if (!shift)
                {
                    MeshManager.Instance.SelectedMeshes.ForEach(m => m.IsSelected = false);
                }
                if (underCursor != null)
                {
                    underCursor.IsActive = true;
                }

                op.Complete();
            }
        }

        /// <summary>
        /// Finds the mesh under a screen position if there is one.
        /// </summary>
        /// <param name="screenPos">The screen space corrdinates in pixels.</param>
        /// <returns>The mesh at the given position, or null of there is none.</returns>
        private MeshInfo GetMeshAtScreenPos(Vector2 screenPos)
        {
            // get the ray pointing along the cursor position on the screen
            Camera cam = m_window.Camera.Camera;
            Vector3 rayOrigin = cam.Transform.Position;
            Vector3 rayDir = cam.ScreenPointToWorldDirection(screenPos);

            MeshInfo underCursor = null;
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

                // first check if the mesh's bounding box is under the cursor since it is way faster
                if (mesh.Bounds.Raycast(orig, dir))
                {
                    Matrix4 toWorld = entity.Transform.LocalToWorldMatix;

                    Vector3[] verts = mesh.Vertices;
                    Triangle[] tris = mesh.Triangles;

                    // raycast each triangle in the mesh and get the intersect if there is a hit
                    for (int i = 0; i < tris.Length; i++)
                    {
                        Triangle tri = tris[i];
                        Vector3 localIntersect;

                        if (Utils.RaycastTriangle(
                            verts[tri.index0],
                            verts[tri.index1],
                            verts[tri.index2],
                            orig, dir, out localIntersect))
                        {
                            // transform the intersect back into world space
                            Vector3 intersect = Vector3.TransformPosition(localIntersect, toWorld);
                            float dist = (intersect - rayOrigin).Length;

                            // select the mesh if close but not before or after the camera clip planes
                            if (dist < minDistance && cam.NearClip < dist && dist < cam.FarClip)
                            {
                                minDistance = dist;
                                underCursor = info;
                            }
                        }
                    }
                }
            }
            return underCursor;
        }

        /// <summary>
        /// Sets the face render modes of all meshes in the scene.
        /// </summary>
        private void SetRenderModeAll()
        {
            // apply the front and backface render modes to the renderers.
            foreach (Transform child in m_meshesRoot.Transform.Children)
            {
                foreach (MeshRenderer renderer in child.Entity.GetComponents<MeshRenderer>())
                {
                    bool isFront = renderer.CullMode == Rendering.CullMode.Back;
                    SetRenderMode(renderer, isFront ? m_frontFaceMode : m_backFaceMode);
                }
            }
        }

        /// <summary>
        /// Sets the face mode for a mesh renderer.
        /// </summary>
        /// <param name="renderer">The mesh renderer to set the mode of.</param>
        /// <param name="mode">The face render mode.</param>
        private void SetRenderMode(MeshRenderer renderer, RenderMode mode)
        {
            switch (mode)
            {
                case RenderMode.Solid:
                    renderer.FaceMode = PolygonMode.Fill;
                    renderer.ShaderProgram = ShaderManager.SHADER_LIT;
                    renderer.Enabled = m_entityToMesh[renderer.Entity].IsVisible;
                    break;
                case RenderMode.Wireframe:
                    renderer.FaceMode = PolygonMode.Line;
                    renderer.ShaderProgram = ShaderManager.SHADER_UNLIT;
                    renderer.Enabled = m_entityToMesh[renderer.Entity].IsVisible;
                    break;
                case RenderMode.Hidden:
                    renderer.Enabled = false;
                    break;
            }
        }
    }
}
