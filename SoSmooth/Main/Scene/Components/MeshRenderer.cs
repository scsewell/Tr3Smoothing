using OpenTK;
using SoSmooth.Rendering;
using SoSmooth.Rendering.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public class MeshRenderer : Renderable
    {
        //private Mesh m_mesh;
        private string m_shaderProgram;

        /// <summary>
        /// The name of the desired shader program.
        /// </summary>
        public string ShaderProgram
        {
            get { return m_shaderProgram; }
            set
            {
                if (m_shaderProgram != value)
                {
                    m_shaderProgram = value;
                    m_program = null;
                }
            }
        }

        private Matrix4Uniform m_modelMatUniform;
        private Matrix4Uniform m_viewMatUniform;
        private Matrix4Uniform m_projMatUniform;

        private ShaderProgram m_program;
        private Surface m_surface;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshRenderer(Entity entity) : base(entity)
        {
            m_modelMatUniform = new Matrix4Uniform("modelMatrix");
            m_viewMatUniform = new Matrix4Uniform("viewMatrix");
            m_projMatUniform = new Matrix4Uniform("projMatrix");
        }
        
        public void SetMesh<TVertex>(Mesh<TVertex> mesh) where TVertex : struct, IVertexData
        {
            m_surface = mesh.ToIndexedSurface<TVertex>();
            m_surface.AddSetting(m_modelMatUniform);
            m_surface.AddSetting(m_viewMatUniform);
            m_surface.AddSetting(m_projMatUniform);

            if (m_program != null)
            {
                m_surface.SetShaderProgram(m_program);
            }
            /*
            if (m_surface != null)
            {
                m_surface.Dispose();
            }
            m_mesh = mesh;
            */
        }

        /// <summary>
        /// Render this mesh.
        /// </summary>
        /// <param name="camera">The camera that is rendering.</param>
        public override void Render(Camera camera)
        {
            if (m_program == null)
            {
                m_program = ShaderManager.Instance.GetProgram(m_shaderProgram);
            }
            if (m_surface == null)
            {

            }

            /*
            if (m_surface == null && m_mesh != null)
            {
                m_surface = m_mesh.ToIndexedSurface<TVertex>();
                m_surface.AddSetting(m_modelMatUniform);
                m_surface.AddSetting(m_viewMatUniform);
                m_surface.AddSetting(m_projMatUniform);

                if (m_program != null)
                {
                    m_surface.SetShaderProgram(m_program);
                }
            }
            */
            if (m_surface != null && m_program != null)
            {
                m_surface.SetShaderProgram(m_program);

                m_modelMatUniform.Matrix = Entity.Transform.LocalToWorldMatix;
                m_viewMatUniform.Matrix = camera.ViewMatrix;
                m_projMatUniform.Matrix = camera.ProjectionMatrix;

                m_surface.Render();
            }
        }
    }
}
