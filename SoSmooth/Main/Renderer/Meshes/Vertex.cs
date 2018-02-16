using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using SoSmooth.Rendering.Vertices;

namespace SoSmooth.Meshes
{
    /// <summary>
    /// A general vertex that can store any per vertex data. This should never be
    /// used directly when rendering, rather cast to a struct that implements 
    /// <see cref="IVertexData"/> and only has the neccessary data fields.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
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
        public Color4 color;
        
        /// <summary>
        /// Creates a new mesh vertex with a given position.
        /// </summary>
        public Vertex(Vector3 position) : this(position, Vector3.Zero, Color4.White) { }

        /// <summary>
        /// Creates a new mesh vertex with a given position and normal.
        /// </summary>
        public Vertex(Vector3 position, Vector3 normal) : this(position, normal, Color4.White) { }

        /// <summary>
        /// Creates a new mesh vertex with a given position and color.
        /// </summary>
        public Vertex(Vector3 position, Color4 color) : this(position, Vector3.Zero, color) { }

        /// <summary>
        /// Creates a new mesh vertex with a given position, normal, and color.
        /// </summary>
        public Vertex(Vector3 position, Vector3 normal, Color4 color)
        {
            this.position = position;
            this.normal = normal;
            this.color = color;
        }

        /// <summary>
        /// Convert to a <see cref="VertexP"/> compact format.
        /// </summary>
        public static VertexP ToVerteP(Vertex v)
        {
            return new VertexP(v.position);
        }

        /// <summary>
        /// Convert to a <see cref="VertexPN"/> compact format.
        /// </summary>
        public static VertexPN ToVertePN(Vertex v)
        {
            return new VertexPN(v.position, v.normal);
        }

        /// <summary>
        /// Convert to a <see cref="VertexPC"/> compact format.
        /// </summary>
        public static VertexPC ToVertePC(Vertex v)
        {
            return new VertexPC(v.position, v.color);
        }

        /// <summary>
        /// Convert to a <see cref="VertexPNC"/> compact format.
        /// </summary>
        public static VertexPNC ToVertePNC(Vertex v)
        {
            return new VertexPNC(v.position, v.normal, v.color);
        }
    }
}
