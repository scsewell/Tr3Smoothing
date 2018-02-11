using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering.Meshes
{
    /// <summary>
    /// A basic vertex for mesh rendering. Consists of a position, normal, and color.
    /// Setting the layout pack to 1 ensures no gaps exist within the structure in memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexNC : IVertexData
    {
        private static readonly int SIZE = VertexData.SizeOf<VertexNC>();
        private static VertexAttribute[] m_attributes;

        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The normal of the vertex.
        /// </summary>
        public Vector3 normal;

        /// <summary>
        /// The color of the vertex.
        /// </summary>
        public Color color;
        
        /// <summary>
        /// Creates a new mesh vertex with a given position.
        /// </summary>
        public VertexNC(Vector3 position) : this(position, new Vector3(0), Color.White)
        { }

        /// <summary>
        /// Creates a new mesh vertex with a given position and normal.
        /// </summary>
        public VertexNC(Vector3 position, Vector3 normal) : this(position, normal, Color.White)
        { }

        /// <summary>
        /// Creates a new mesh vertex with a given position and color.
        /// </summary>
        public VertexNC(Vector3 position, Color color) : this(position, new Vector3(0), color)
        { }

        /// <summary>
        /// Creates a new mesh vertex with a given position, normal, and color.
        /// </summary>
        public VertexNC(Vector3 position, Vector3 normal, Color color)
        {
            this.position = position;
            this.normal = normal;
            this.color = color;
        }

        public int Size()
        {
            return SIZE;
        }

        public VertexAttribute[] VertexAttributes()
        {
            return m_attributes ?? (m_attributes = MakeVertexArray());
        }

        private static VertexAttribute[] MakeVertexArray()
        {
            return VertexData.MakeAttributeArray(
                VertexData.MakeAttributeTemplate<Vector3>("v_position"),
                VertexData.MakeAttributeTemplate<Vector3>("v_normal"),
                VertexData.MakeAttributeTemplate<Color>("v_color")
            );
        }
    }
}