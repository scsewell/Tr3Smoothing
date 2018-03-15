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

        /// <summary>
        /// Called when the mesh has changed.
        /// </summary>
        protected override void OnMeshChanged()
        {
            ReadOnlyCollection<Vector3> corners = m_mesh.Bounds.Corners;

            // clear any lines
            m_vertexBuffer.Clear();

            // get the underlying buffer
            int offset;
            VertexPNC[] verts = m_vertexBuffer.WriteDirectly(24, out offset);

            // each consecutive pair of elements forms a line segment drawn to form the box wireframe
            verts[0]    = new VertexPNC(corners[0], Vector3.Zero, Color4.White);
            verts[1]    = new VertexPNC(corners[4], Vector3.Zero, Color4.White);
            verts[2]    = new VertexPNC(corners[1], Vector3.Zero, Color4.White);
            verts[3]    = new VertexPNC(corners[5], Vector3.Zero, Color4.White);
            verts[4]    = new VertexPNC(corners[2], Vector3.Zero, Color4.White);
            verts[5]    = new VertexPNC(corners[6], Vector3.Zero, Color4.White);
            verts[6]    = new VertexPNC(corners[3], Vector3.Zero, Color4.White);
            verts[7]    = new VertexPNC(corners[7], Vector3.Zero, Color4.White);

            verts[8]    = new VertexPNC(corners[0], Vector3.Zero, Color4.White);
            verts[9]    = new VertexPNC(corners[1], Vector3.Zero, Color4.White);
            verts[10]   = new VertexPNC(corners[1], Vector3.Zero, Color4.White);
            verts[11]   = new VertexPNC(corners[3], Vector3.Zero, Color4.White);
            verts[12]   = new VertexPNC(corners[3], Vector3.Zero, Color4.White);
            verts[13]   = new VertexPNC(corners[2], Vector3.Zero, Color4.White);
            verts[14]   = new VertexPNC(corners[2], Vector3.Zero, Color4.White);
            verts[15]   = new VertexPNC(corners[0], Vector3.Zero, Color4.White);

            verts[16]   = new VertexPNC(corners[4], Vector3.Zero, Color4.White);
            verts[17]   = new VertexPNC(corners[5], Vector3.Zero, Color4.White);
            verts[18]   = new VertexPNC(corners[5], Vector3.Zero, Color4.White);
            verts[19]   = new VertexPNC(corners[7], Vector3.Zero, Color4.White);
            verts[20]   = new VertexPNC(corners[7], Vector3.Zero, Color4.White);
            verts[21]   = new VertexPNC(corners[6], Vector3.Zero, Color4.White);
            verts[22]   = new VertexPNC(corners[6], Vector3.Zero, Color4.White);
            verts[23]   = new VertexPNC(corners[4], Vector3.Zero, Color4.White);
            
            // upload the lines to the GPU
            m_vertexBuffer.BufferData();
        }

        /// <summary>
        /// Disposes this component and frees held resources
        /// </summary>
        /// <param name="entity">True if managed resources should be cleaned up.</param>
        protected override void OnDispose(bool disposing)
        {
            m_vertexBuffer.Dispose();

            base.OnDispose(disposing);
        }
    }
}
