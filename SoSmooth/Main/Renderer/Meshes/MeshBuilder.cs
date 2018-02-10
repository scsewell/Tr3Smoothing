using System.Collections.Generic;

namespace SoSmooth.Renderer.Meshes
{
    /// <summary>
    /// A mutable builder class to help construct immutable meshes.
    /// </summary>
    public class MeshBuilder<TVertex> where TVertex : struct
    {
        private readonly List<TVertex> m_vertices;
        private readonly List<IndexTriangle> m_triangles;

        /// <summary>
        /// Creates a new mesh builder.
        /// If known, specifying the vertex and/or triangle capacity
        /// will prevent additional memory allocations for better performance.
        /// </summary>
        public MeshBuilder(int vertexCapacity = 0, int triangleCapacity = 0)
        {
            m_vertices = new List<TVertex>(vertexCapacity);
            m_triangles = new List<IndexTriangle>(triangleCapacity);
        }

        /// <summary>
        /// Builds and return a mesh with the builder's data.
        /// </summary>
        public Mesh<TVertex> Build()
        {
            return new Mesh<TVertex>(m_vertices.ToArray(), m_triangles.ToArray());
        }

        /// <summary>
        /// Adds a vertex to the builder.
        /// </summary>
        /// <returns>The index of the vertex for triangle creation.</returns>
        public int AddVertex(TVertex v)
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
        public int AddVertices(TVertex v0, TVertex v1)
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
        public int AddVertices(TVertex v0, TVertex v1, TVertex v2)
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
        public int AddVertices(TVertex v0, TVertex v1, TVertex v2, TVertex v3)
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
        public int AddVertices(params TVertex[] vs)
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
        public int AddVertices(IEnumerable<TVertex> vs)
        {
            int i = m_vertices.Count;
            m_vertices.AddRange(vs);
            return i;
        }

        /// <summary>
        /// Adds a triangle to the builder.
        /// </summary>
        public void AddTriangle(IndexTriangle t)
        {
            m_triangles.Add(t);
        }

        /// <summary>
        /// Adds triangles to the builder.
        /// </summary>
        public void AddTriangles(IndexTriangle t0, IndexTriangle t1)
        {
            m_triangles.Add(t0);
            m_triangles.Add(t1);
        }

        /// <summary>
        /// Adds triangles to the builder.
        /// </summary>
        public void AddTriangles(params IndexTriangle[] ts)
        {
            m_triangles.AddRange(ts);
        }

        /// <summary>
        /// Adds triangles to the builder.
        /// </summary>
        public void AddTriangles(IEnumerable<IndexTriangle> ts)
        {
            m_triangles.AddRange(ts);
        }
    }
}