using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering.Vertices
{
    /// <summary>
    /// A vertex for mesh rendering. Consists of a position and normal.
    /// </summary>
    /// <remarks>
    /// The field names must match those declared in the vertex shaders.
    /// The struct layout pack = 1 is essential, otherwise there may
    /// be gaps in the struct in memory.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPN : IVertexData
    {
        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 v_position;

        /// <summary>
        /// The normal of the vertex.
        /// </summary>
        public Int2101010 v_normal;
        
        /// <summary>
        /// Creates a new mesh vertex with a given position and normal.
        /// </summary>
        public VertexPN(Vector3 position, Vector3 normal)
        {
            v_position = position;
            v_normal = new Int2101010(normal);
        }
    }
}