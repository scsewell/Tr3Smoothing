using SoSmooth.Rendering;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public sealed class MeshRenderer : Renderable
    {
        private IndexedSurface m_surface;
        protected override Surface Surface => m_surface;

        private readonly Matrix4Uniform m_modelMatUniform = new Matrix4Uniform("modelMatrix");
        private readonly Matrix4Uniform m_viewMatUniform = new Matrix4Uniform("viewMatrix");
        private readonly Matrix4Uniform m_projMatUniform = new Matrix4Uniform("projMatrix");

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
        public MeshRenderer(Entity entity) : base(entity)
        {
            m_surface = new IndexedSurface();

            m_surface.AddSetting(m_cullMode);
            m_surface.AddSetting(m_polygonMode);
            m_surface.AddSetting(m_depthMask);
            m_surface.AddSetting(m_blend);

            m_surface.AddSetting(m_color);

            m_surface.AddSetting(m_modelMatUniform);
            m_surface.AddSetting(m_viewMatUniform);
            m_surface.AddSetting(m_projMatUniform);
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
                m_surface.SetVertexBuffer(m_mesh.VBO);
                m_surface.SetIndexBuffer(m_mesh.IBO);

                m_modelMatUniform.Value = Entity.Transform.LocalToWorldMatix;
                m_viewMatUniform.Value = camera.ViewMatrix;
                m_projMatUniform.Value = camera.ProjectionMatrix;
                    
                m_surface.Render();
            }
        }
    }
}
