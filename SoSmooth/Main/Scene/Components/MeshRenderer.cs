using SoSmooth.Rendering;
using SoSmooth.Meshes;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that handles the rendering of a mesh.
    /// </summary>
    public sealed class MeshRenderer : MeshBasedRenderer
    {
        private readonly IndexedSurface m_surface;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshRenderer(Entity entity, Mesh mesh) : base(entity, mesh, new IndexedSurface())
        {
            m_surface = Surface as IndexedSurface;
        }
        
        /// <summary>
        /// Render this mesh.
        /// </summary>
        /// <param name="camera">The camera that is rendering.</param>
        protected override void OnRender(Camera camera)
        {
            m_surface.SetVertexBuffer(m_mesh.VBO);
            m_surface.SetIndexBuffer(m_mesh.IBO);
        }
    }
}
