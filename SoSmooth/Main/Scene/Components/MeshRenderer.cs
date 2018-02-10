using SoSmooth.Renderer;
using SoSmooth.Renderer.Meshes;
using OpenTK;

namespace SoSmooth.Scene
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public class MeshRenderer : Component, IRenderable
    {
        private ShaderProgram m_program;
        private Surface m_surface;

        private Matrix4Uniform m_modelMatUniform;
        private Matrix4Uniform m_viewMatUniform;
        private Matrix4Uniform m_projMatUniform;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshRenderer(Entity entity) : base(entity)
        {
            m_modelMatUniform = new Matrix4Uniform("modelMatrix");
            m_viewMatUniform = new Matrix4Uniform("viewMatrix");
            m_projMatUniform = new Matrix4Uniform("projMatrix");
        }

        public void SetProgram(ShaderProgram program)
        {
            m_program = program;

            if (m_surface != null)
            {
                m_surface.SetShaderProgram(m_program);
            }
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
        }

        /// <summary>
        /// Render this mesh.
        /// </summary>
        /// <param name="camera">The camera that is rendering.</param>
        public void Render(Camera camera)
        {
            if (m_surface != null && m_program != null)
            {
                m_modelMatUniform.Matrix = Entity.Transform.LocalToWorldMatix;
                m_viewMatUniform.Matrix = camera.ViewMatrix;
                m_projMatUniform.Matrix = camera.ProjectionMatrix;
                
                m_surface.Render();
            }
        }
    }
}
