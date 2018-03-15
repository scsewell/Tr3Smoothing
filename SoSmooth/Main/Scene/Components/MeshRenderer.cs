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
        protected override void OnRender()
        {
            m_surface.SetVertexBuffer(m_mesh.VBO);
            m_surface.SetIndexBuffer(m_mesh.IBO);
        }
    }
}
