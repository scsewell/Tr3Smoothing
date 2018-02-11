using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering.Meshes
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
    public struct VertexNC : IVertexData
    {
        private static readonly int m_size = Marshal.SizeOf<VertexNC>();
        private static readonly VertexAttribute[] m_attributes = VertexData.MakeAttributeArray<VertexNC>();

        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 v_position;

        /// <summary>
        /// The normal of the vertex.
        /// </summary>
        public Vector3h v_normal;

        /// <summary>
        /// The color of the vertex.
        /// </summary>
        public Color v_color;
        
        /// <summary>
        /// Creates a new mesh vertex with a given position.
        /// </summary>
        public VertexNC(Vector3 position) : this(position, Vector3.Zero, Color.White)
        { }

        /// <summary>
        /// Creates a new mesh vertex with a given position and normal.
        /// </summary>
        public VertexNC(Vector3 position, Vector3 normal) : this(position, normal, Color.White)
        { }

        /// <summary>
        /// Creates a new mesh vertex with a given position and color.
        /// </summary>
        public VertexNC(Vector3 position, Color color) : this(position, Vector3.Zero, color)
        { }

        /// <summary>
        /// Creates a new mesh vertex with a given position, normal, and color.
        /// </summary>
        public VertexNC(Vector3 position, Vector3 normal, Color color)
        {
            v_position = position;
            v_normal = new Vector3h(normal);
            v_color = color;
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