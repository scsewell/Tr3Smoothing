using System.Collections.ObjectModel;
using OpenTK;
using OpenTK.Graphics;
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
        private readonly VertexBuffer<VertexPNC> m_vertexBuffer;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public BoundsRenderer(Entity entity, Mesh mesh) : base(entity, mesh, new VertexSurface())
        {
            m_vertexBuffer = new VertexBuffer<VertexPNC>(24);

            m_surface = Surface as VertexSurface;
            m_surface.SetVertexBuffer(m_vertexBuffer, PrimitiveType.Lines);
            
            ShaderProgram = ShaderManager.SHADER_UNLIT;
            BlendMode = BlendMode.Alpha;

            OnMeshChanged();
        }

        protected override void OnMeshChanged()
        {
            ReadOnlyCollection<Vector3> corners = m_mesh.Bounds.Corners;

            // clear any lines
            m_vertexBuffer.Clear();

            // each consecutive pair of elements forms a line segment drawn to form the box wireframe
            m_vertexBuffer.AddElement(new VertexPNC(corners[0], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[4], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[1], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[5], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[2], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[6], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[3], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[7], Vector3.Zero, Color4.White));

            m_vertexBuffer.AddElement(new VertexPNC(corners[0], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[1], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[1], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[3], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[3], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[2], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[2], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[0], Vector3.Zero, Color4.White));

            m_vertexBuffer.AddElement(new VertexPNC(corners[4], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[5], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[5], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[7], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[7], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[6], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[6], Vector3.Zero, Color4.White));
            m_vertexBuffer.AddElement(new VertexPNC(corners[4], Vector3.Zero, Color4.White));

            // upload the lines to the GPU
            m_vertexBuffer.BufferData();
        }
    }
}
