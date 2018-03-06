using System.Collections.ObjectModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;
using SoSmooth.Rendering.Vertices;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh's bounds.
    /// </summary>
    public sealed class BoundsRenderer : MeshBasedRenderer
    {
        private readonly VertexSurface m_surface;
        private readonly VertexBuffer<VertexP> m_vertexBuffer;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public BoundsRenderer(Entity entity, Mesh mesh) : base(entity, mesh, new VertexSurface())
        {
            m_vertexBuffer = new VertexBuffer<VertexP>(24);

            m_surface = Surface as VertexSurface;
            m_surface.SetVertexBuffer(m_vertexBuffer, PrimitiveType.Lines);

            Color = Random.GetColor();
            BlendMode = BlendMode.Alpha;

            ShaderProgram program;
            ShaderManager.Instance.GetProgram(ShaderManager.SHADER_UNLIT, out program);
            Surface.SetShaderProgram(program);

            OnMeshChanged();
        }

        protected override void OnMeshChanged()
        {
            ReadOnlyCollection<Vector3> corners = m_mesh.Bounds.Corners;

            // clear any lines
            m_vertexBuffer.Clear();

            // each consecutive pair of elements forms a line segment drawn to form the box wireframe
            m_vertexBuffer.AddElement(new VertexP(corners[0]));
            m_vertexBuffer.AddElement(new VertexP(corners[4]));
            m_vertexBuffer.AddElement(new VertexP(corners[1]));
            m_vertexBuffer.AddElement(new VertexP(corners[5]));
            m_vertexBuffer.AddElement(new VertexP(corners[2]));
            m_vertexBuffer.AddElement(new VertexP(corners[6]));
            m_vertexBuffer.AddElement(new VertexP(corners[3]));
            m_vertexBuffer.AddElement(new VertexP(corners[7]));

            m_vertexBuffer.AddElement(new VertexP(corners[0]));
            m_vertexBuffer.AddElement(new VertexP(corners[1]));
            m_vertexBuffer.AddElement(new VertexP(corners[1]));
            m_vertexBuffer.AddElement(new VertexP(corners[3]));
            m_vertexBuffer.AddElement(new VertexP(corners[3]));
            m_vertexBuffer.AddElement(new VertexP(corners[2]));
            m_vertexBuffer.AddElement(new VertexP(corners[2]));
            m_vertexBuffer.AddElement(new VertexP(corners[0]));

            m_vertexBuffer.AddElement(new VertexP(corners[4]));
            m_vertexBuffer.AddElement(new VertexP(corners[5]));
            m_vertexBuffer.AddElement(new VertexP(corners[5]));
            m_vertexBuffer.AddElement(new VertexP(corners[7]));
            m_vertexBuffer.AddElement(new VertexP(corners[7]));
            m_vertexBuffer.AddElement(new VertexP(corners[6]));
            m_vertexBuffer.AddElement(new VertexP(corners[6]));
            m_vertexBuffer.AddElement(new VertexP(corners[4]));

            // upload the lines to the GPU
            m_vertexBuffer.BufferData();
        }
    }
}
