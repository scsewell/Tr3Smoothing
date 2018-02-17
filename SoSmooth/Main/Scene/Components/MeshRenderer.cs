using SoSmooth.Rendering;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public class MeshRenderer : Renderable
    {
        private IndexedSurface m_surface;
        protected override Surface Surface => m_surface;

        private Matrix4Uniform m_modelMatUniform;
        private Matrix4Uniform m_viewMatUniform;
        private Matrix4Uniform m_projMatUniform;

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

            m_modelMatUniform = new Matrix4Uniform("modelMatrix");
            m_viewMatUniform = new Matrix4Uniform("viewMatrix");
            m_projMatUniform = new Matrix4Uniform("projMatrix");

            m_surface.AddSetting(m_modelMatUniform);
            m_surface.AddSetting(m_viewMatUniform);
            m_surface.AddSetting(m_projMatUniform);
        }
        
        /// <summary>
        /// Render this mesh.
        /// </summary>
        /// <param name="camera">The camera that is rendering.</param>
        protected override void OnRender(Camera camera)
        {
            if (m_mesh != null)
            {
                m_surface.SetVertexBuffer(m_mesh.VertexBuffer);
                m_surface.SetIndexBuffer(m_mesh.IndexBuffer);

                m_modelMatUniform.Value = Entity.Transform.LocalToWorldMatix;
                m_viewMatUniform.Value = camera.ViewMatrix;
                m_projMatUniform.Value = camera.ProjectionMatrix;
                    
                m_surface.Render();
            }
        }
    }
}
