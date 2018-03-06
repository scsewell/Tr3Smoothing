using SoSmooth.Rendering;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Base class for renderer components that use meshes when rendering.
    /// </summary>
    public abstract class MeshBasedRenderer : Renderer
    {
        protected Mesh m_mesh;
        protected Bounds m_bounds;
        private bool m_boundsDirty = true;

        /// <summary>
        /// The mesh to be rendered.
        /// </summary>
        public Mesh Mesh
        {
            get { return m_mesh; }
            set
            {
                if (m_mesh != value)
                {
                    m_mesh = value;
                    m_boundsDirty = true;
                    OnMeshChanged();
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshBasedRenderer(Entity entity, Mesh mesh, Surface surface) : base(entity, surface)
        {
            m_mesh = mesh;

            Transform.Moved += OnMeshMoved;
        }

        /// <summary>
        /// Called when the mesh has been moved in the world.
        /// </summary>
        private void OnMeshMoved()
        {
            m_boundsDirty = true;
        }

        /// <summary>
        /// Checks if this component is culled.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        protected override bool OnCull(Camera camera)
        {
            if (m_mesh == null)
            {
                return true;
            }

            // Get the axis aligned bounding box for the mesh in the scene
            if (m_boundsDirty)
            {
                m_bounds = m_mesh.Bounds.Transformed(Transform.LocalToWorldMatix);
                m_boundsDirty = false;
            }

            // only render the mesh if it's bounds are contained by the camera frustum
            return !m_bounds.InFrustum(camera.FrustumPlanes);
        }

        /// <summary>
        /// Called when the mesh for this renderer has changed.
        /// </summary>
        protected virtual void OnMeshChanged() { }
    }
}
