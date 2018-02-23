using SoSmooth.Rendering;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public sealed class MeshRenderer : Renderable
    {
        private readonly Matrix4Uniform m_modelMatUniform = new Matrix4Uniform("u_modelMatrix");

        private Mesh m_mesh;

        /// <summary>
        /// The mesh to be rendered.
        /// </summary>
        public Mesh Mesh
        {
            get { return m_mesh; }
            set { m_mesh = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshRenderer(Entity entity) : base(entity, new IndexedSurface())
        {
            Surface.AddSetting(m_modelMatUniform);
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
            return false;
        }
        
        /// <summary>
        /// Render this mesh.
        /// </summary>
        /// <param name="camera">The camera that is rendering.</param>
        protected override void OnRender(Camera camera)
        {
            if (m_mesh != null)
            {
                IndexedSurface surface = Surface as IndexedSurface;

                surface.SetVertexBuffer(m_mesh.VBO);
                surface.SetIndexBuffer(m_mesh.IBO);

                m_modelMatUniform.Value = Entity.Transform.LocalToWorldMatix;
                
                surface.Render();
            }
        }
    }
}
