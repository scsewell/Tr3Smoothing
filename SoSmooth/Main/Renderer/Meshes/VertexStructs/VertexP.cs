using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering.Meshes.Vertices
{
    /// <summary>
    /// A vertex for mesh rendering. Consists of a position.
    /// </summary>
    /// <remarks>
    /// The field names must match those declared in the vertex shaders.
    /// The struct layout pack = 1 is essential, otherwise there may
    /// be gaps in the struct in memory.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexP : IVertexData
    {
        private static readonly int m_size = Marshal.SizeOf<VertexP>();
        private static readonly VertexAttribute[] m_attributes = VertexData.MakeAttributeArray<VertexP>();

        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 v_position;
        
        /// <summary>
        /// Creates a new mesh vertex with a given position.
        /// </summary>
        public VertexP(Vector3 position)
        {
            v_position = position;
        }

        public int Size()
        {
            return m_size;
        }

        public VertexAttribute[] VertexAttributes()
        {
            return m_attributes;
        }
    }
}