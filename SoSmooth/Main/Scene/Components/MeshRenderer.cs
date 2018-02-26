using SoSmooth.Rendering;
using SoSmooth.Meshes;
using SoSmooth.Rendering.Vertices;

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

        VertexSurface surf;
        VertexBuffer<VertexP> vertexBuffer;

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

            Bounds bounds = m_mesh.BoundingBox.Transformed(Transform.LocalToWorldMatix);

            camera.FrustumPlanes.GetType();

            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer<VertexP>();

                surf = new VertexSurface();
                surf.AddSetting(m_modelMatUniform);
                surf.SetVertexBuffer(vertexBuffer, OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
                ShaderProgram program;
                ShaderManager.Instance.GetProgram(ShaderManager.SHADER_UNLIT, out program);
                surf.SetShaderProgram(program);
            }

            vertexBuffer.Clear();
            vertexBuffer.AddElement(new VertexP(bounds.Corners[0]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[4]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[1]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[5]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[2]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[6]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[3]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[7]));
            
            vertexBuffer.AddElement(new VertexP(bounds.Corners[0]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[1]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[1]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[3]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[3]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[2]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[2]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[0]));

            vertexBuffer.AddElement(new VertexP(bounds.Corners[4]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[5]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[5]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[7]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[7]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[6]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[6]));
            vertexBuffer.AddElement(new VertexP(bounds.Corners[4]));
            vertexBuffer.BufferData();
            return !bounds.InFrustum(camera.FrustumPlanes);
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

                m_modelMatUniform.Value = OpenTK.Matrix4.Identity;
                surf.Render();
            }
        }
    }
}
