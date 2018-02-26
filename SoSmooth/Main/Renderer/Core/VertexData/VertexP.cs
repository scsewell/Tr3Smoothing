using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering.Vertices
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
    }
}