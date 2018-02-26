using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace SoSmooth.Rendering.Vertices
{
    /// <summary>
    /// A vertex for mesh rendering. Consists of a position, normal, and color.
    /// </summary>
    /// <remarks>
    /// The field names must match those declared in the vertex shaders.
    /// The struct layout pack = 1 is essential, otherwise there may
    /// be gaps in the struct in memory.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPNC : IVertexData
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
        /// The color of the vertex.
        /// </summary>
        public Color v_color;
        
        /// <summary>
        /// Creates a new mesh vertex with a given position, normal, and color.
        /// </summary>
        public VertexPNC(Vector3 position, Vector3 normal, Color4 color)
        {
            v_position = position;
            v_normal = new Int2101010(normal);
            v_color = new Color(color);
        }
    }
}