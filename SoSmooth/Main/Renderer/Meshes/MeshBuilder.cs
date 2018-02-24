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
        private readonly List<Vertex> m_vertices;
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
            m_vertices = new List<Vertex>(vertexCapacity);
            m_triangles = new List<Triangle>(triangleCapacity);
        }

        /// <summary>
        /// Builds and returns a mesh using the builder's current data.
        /// </summary>
        /// <param name="name">The name of the mesh.</param>
        /// <param name="useNormals">Allow rendering with normals.</param>
        /// <param name="useColors">Allow rendering with vertex color.</param>
        public Mesh CreateMesh(string name, bool useNormals, bool useColors)
        {
            return new Mesh(
                name,
                m_vertices.ToArray(), 
                m_triangles.ToArray(),
                useNormals,
                useColors);
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the vertex for triangle creation.</returns>
        public int AddVertex(Vertex v)
        {
            int i = m_vertices.Count;
            m_vertices.Add(v);
            return i;
        }
        
        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the first vertex for triangle creation.
        /// Indices for following vertices incrent by 1 each.</returns>
        public int AddVertices(Vertex v0, Vertex v1)
        {
            int i = m_vertices.Count;
            m_vertices.Add(v0);
            m_vertices.Add(v1);
            return i;
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the first vertex for triangle creation.
        /// Indices for following vertices incrent by 1 each.</returns>
        public int AddVertices(Vertex v0, Vertex v1, Vertex v2)
        {
            int i = m_vertices.Count;
            m_vertices.Add(v0);
            m_vertices.Add(v1);
            m_vertices.Add(v2);
            return i;
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the first vertex for triangle creation.
        /// Indices for following vertices incrent by 1 each.</returns>
        public int AddVertices(Vertex v0, Vertex v1, Vertex v2, Vertex v3)
        {
            int i = m_vertices.Count;
            m_vertices.Add(v0);
            m_vertices.Add(v1);
            m_vertices.Add(v2);
            m_vertices.Add(v3);
            return i;
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the first vertex for triangle creation.
        /// Indices for following vertices incrent by 1 each.</returns>
        public int AddVertices(params Vertex[] vs)
        {
            int i = m_vertices.Count;
            m_vertices.AddRange(vs);
            return i;
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the first vertex for triangle creation.
        /// Indices for following vertices incrent by 1 each.</returns>
        public int AddVertices(IEnumerable<Vertex> vs)
        {
            int i = m_vertices.Count;
            m_vertices.AddRange(vs);
            return i;
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

            m_builder.AddVertex(new Vertex(new Vector3(-u, -u, -u), Color4.Brown));
            m_builder.AddVertex(new Vertex(new Vector3(u, -u, -u),  Color4.Red));
            m_builder.AddVertex(new Vertex(new Vector3(u, u, -u),   Color4.Orange));
            m_builder.AddVertex(new Vertex(new Vector3(-u, u, -u),  Color4.Yellow));

            m_builder.AddVertex(new Vertex(new Vector3(-u, -u, u),  Color4.Green));
            m_builder.AddVertex(new Vertex(new Vector3(u, -u, u),   Color4.Cyan));
            m_builder.AddVertex(new Vertex(new Vector3(u, u, u),    Color4.Blue));
            m_builder.AddVertex(new Vertex(new Vector3(-u, u, u),   Color4.Magenta));

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

            Mesh mesh = m_builder.CreateMesh("Cube", true, true);
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}