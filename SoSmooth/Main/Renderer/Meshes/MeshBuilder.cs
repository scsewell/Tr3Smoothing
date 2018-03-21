using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace SoSmooth.Meshes
{
    /// <summary>
    /// A builder class to help construct meshes.
    /// </summary>
    public class MeshBuilder
    {
        private readonly List<Vector3> m_vertices;
        private readonly List<Vector3> m_normals;
        private readonly List<Color4> m_colors;
        private readonly List<Triangle> m_triangles;

        /// <summary>
        /// The number of vertices added to the current mesh.
        /// </summary>
        public int VertexCount => m_vertices.Count;

        /// <summary>
        /// The number of triangles added to the current mesh.
        /// </summary>
        public int TriangleCount => m_triangles.Count;

        /// <summary>
        /// Creates a new mesh builder.
        /// If known, specifying the vertex and/or triangle capacity
        /// will prevent additional memory allocations for better performance.
        /// </summary>
        public MeshBuilder(int vertexCapacity = 0, int triangleCapacity = 0)
        {
            m_vertices = new List<Vector3>(vertexCapacity);
            m_normals = new List<Vector3>(vertexCapacity);
            m_colors = new List<Color4>(vertexCapacity);
            m_triangles = new List<Triangle>(triangleCapacity);
        }

        /// <summary>
        /// Builds and returns a mesh using the builder's current data.
        /// </summary>
        /// <param name="name">The name of the mesh.</param>
        public Mesh CreateMesh(string name)
        {
            return new Mesh(
                name,
                m_vertices.ToArray(),
                m_normals.Count == 0 ? null : m_normals.ToArray(),
                m_colors.Count == 0 ? null : m_colors.ToArray(),
                m_triangles.ToArray()
            );
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        public void AddVertex(Vector3 position)
        {
            m_vertices.Add(position);
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        public void AddVertex(Vector3 position, Color4 color)
        {
            m_vertices.Add(position);
            m_colors.Add(color);
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        public void AddVertex(Vector3 position, Vector3 normal)
        {
            m_vertices.Add(position);
            m_normals.Add(normal);
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        public void AddVertex(Vector3 position, Vector3 normal, Color4 color)
        {
            m_vertices.Add(position);
            m_normals.Add(normal);
            m_colors.Add(color);
        }
        
        /// <summary>
        /// Adds a triangle to the builder.
        /// </summary>
        public void AddTriangle(Triangle t)
        {
            m_triangles.Add(t);
        }

        /// <summary>
        /// Adds triangles to the builder.
        /// </summary>
        public void AddTriangles(Triangle t0, Triangle t1)
        {
            m_triangles.Add(t0);
            m_triangles.Add(t1);
        }

        /// <summary>
        /// Adds triangles to the builder.
        /// </summary>
        public void AddTriangles(params Triangle[] ts)
        {
            m_triangles.AddRange(ts);
        }

        /// <summary>
        /// Adds triangles to the builder.
        /// </summary>
        public void AddTriangles(IEnumerable<Triangle> ts)
        {
            m_triangles.AddRange(ts);
        }

        /// <summary>
        /// Clears all verted and triangle data.
        /// </summary>
        public void Clear()
        {
            m_vertices.Clear();
            m_triangles.Clear();
        }

        private static MeshBuilder m_builder = new MeshBuilder();

        /// <summary>
        /// Returns a simple axis-aligned cube centered around (0, 0, 0).
        /// All edges are length 1.
        /// </summary>
        public static Mesh CreateCube()
        {
            const float u = 0.5f;

            m_builder.Clear();

            m_builder.AddVertex(new Vector3(-u, -u, -u));
            m_builder.AddVertex(new Vector3( u, -u, -u));
            m_builder.AddVertex(new Vector3( u,  u, -u));
            m_builder.AddVertex(new Vector3(-u,  u, -u));

            m_builder.AddVertex(new Vector3(-u, -u, u));
            m_builder.AddVertex(new Vector3( u, -u, u));
            m_builder.AddVertex(new Vector3( u,  u, u));
            m_builder.AddVertex(new Vector3(-u,  u, u));

            m_builder.AddTriangle(new Triangle(0, 3, 2));
            m_builder.AddTriangle(new Triangle(0, 2, 1));
            m_builder.AddTriangle(new Triangle(0, 1, 5));
            m_builder.AddTriangle(new Triangle(0, 5, 4));
            m_builder.AddTriangle(new Triangle(0, 4, 7));
            m_builder.AddTriangle(new Triangle(0, 7, 3));
            m_builder.AddTriangle(new Triangle(6, 5, 1));
            m_builder.AddTriangle(new Triangle(6, 1, 2));
            m_builder.AddTriangle(new Triangle(6, 2, 3));
            m_builder.AddTriangle(new Triangle(6, 3, 7));
            m_builder.AddTriangle(new Triangle(6, 7, 4));
            m_builder.AddTriangle(new Triangle(6, 4, 5));
            
            return m_builder.CreateMesh("Cube");
        }
    }
}