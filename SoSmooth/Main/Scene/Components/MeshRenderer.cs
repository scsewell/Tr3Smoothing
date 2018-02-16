using SoSmooth.Rendering;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public class MeshRenderer : Renderable
    {
        private Mesh m_mesh;
        private string m_programName;

        /// <summary>
        /// The mesh to be rendered.
        /// </summary>
        public Mesh Mesh
        {
            get { return m_mesh; }
            set { m_mesh = value; }
        }

        /// <summary>
        /// The name of the desired shader program.
        /// </summary>
        public string ShaderProgram
        {
            get { return m_programName; }
            set
            {
                if (m_programName != value)
                {
                    m_programName = value;
                    m_shaderProgram = null;
                }
            }
        }

        private Matrix4Uniform m_modelMatUniform;
        private Matrix4Uniform m_viewMatUniform;
        private Matrix4Uniform m_projMatUniform;

        private ShaderProgram m_shaderProgram;
        private IndexedSurface m_surface;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshRenderer(Entity entity) : base(entity)
        {
            m_modelMatUniform = new Matrix4Uniform("modelMatrix");
            m_viewMatUniform = new Matrix4Uniform("viewMatrix");
            m_projMatUniform = new Matrix4Uniform("projMatrix");

            m_surface = new IndexedSurface();
            m_surface.AddSetting(m_modelMatUniform);
            m_surface.AddSetting(m_viewMatUniform);
            m_surface.AddSetting(m_projMatUniform);
        }
        
        /// <summary>
        /// Render this mesh.
        /// </summary>
        /// <param name="camera">The camera that is rendering.</param>
        public override void Render(Camera camera)
        {
            if (m_mesh != null && m_programName != null)
            {
                if (m_shaderProgram == null)
                {
                    m_shaderProgram = ShaderManager.Instance.GetProgram(m_programName);
                }

                if (m_surface != null && m_shaderProgram != null)
                {
                    m_surface.SetShaderProgram(m_shaderProgram);
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
}
