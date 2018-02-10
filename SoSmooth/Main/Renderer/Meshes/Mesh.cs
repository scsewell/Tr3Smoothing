using System;
using OpenTK;

namespace SoSmooth.Renderer.Meshes
{
    /// <summary>
    /// Represents an immutable mesh of vertices and triangles.
    /// </summary>
    public sealed class Mesh<TVertex> where TVertex : struct
    {
        private readonly TVertex[] m_vertices;
        private readonly IndexTriangle[] m_triangles;

        public Mesh(TVertex[] vertices, IndexTriangle[] triangles)
        {
            m_vertices = vertices;
            m_triangles = triangles;
        }

        /// <summary>
        /// Converts the mesh into a renderable indexed surface.
        /// </summary>
        /// <param name="transform">A function to apply to all the vertices.</param>
        public IndexedSurface<TVertexOut> ToIndexedSurface<TVertexOut>(
            Func<TVertex, TVertexOut> transform = null
            ) where TVertexOut : struct, IVertexData
        {
            var surface = new IndexedSurface<TVertexOut>();
            
            WriteVertices(surface, transform);
            WriteIndices(surface);

            return surface;
        }

        /// <summary>
        /// Converts the mesh into a renderable surface as a point cloud. Only vertices and no triangles are included.
        /// </summary>
        /// <param name="transform">A function to apply to all the vertices.</param>
        public VertexSurface<TVertexOut> ToPointCloudSurface<TVertexOut>(
            Func<TVertex, TVertexOut> transform = null
            ) where TVertexOut : struct, IVertexData
        {
            var surface = new VertexSurface<TVertexOut>();

            WriteVertices(surface, transform);

            return surface;
        }

        private void WriteVertices<TVertexOut>(
            VertexSurface<TVertexOut> surface,
            Func<TVertex, TVertexOut> transform = null
            ) where TVertexOut : struct, IVertexData
        {
            ushort offset;
            TVertexOut[] vertexArray = surface.WriteVerticesDirectly(m_vertices.Length, out offset);

            if (offset != 0)
            {
                throw new Exception("Expected vertex offset to be zero.");
            }

            if (transform == null)
            {
                m_vertices.CopyTo(vertexArray, 0);
            }
            else
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    vertexArray[i] = transform(m_vertices[i]);
                }
            }
        }

        private void WriteIndices<TVertexOut>(IndexedSurface<TVertexOut> surface) where TVertexOut : struct, IVertexData
        {
            int offset;
            ushort[] indexArray = surface.WriteIndicesDirectly(m_triangles.Length * 3, out offset);

            if (offset != 0)
            {
                // test not strictly speaking necessary, but enforces usage
                throw new Exception("Expected index offset to be zero.");
            }

            for (int i = 0; i < m_triangles.Length; i++)
            {
                IndexTriangle triangle = m_triangles[i];

                indexArray[offset] = triangle.index0;
                indexArray[offset + 1] = triangle.index1;
                indexArray[offset + 2] = triangle.index2;

                offset += 3;
            }
        }

        /// <summary>
        /// Returns a simple axis-aligned cube centered around (0, 0, 0).
        /// All edges are length 1.
        /// </summary>
        public static Mesh<VertexNC> CreateCube()
        {
            const float u = 0.5f;

            return new Mesh<VertexNC>(
                new[]
                {
                    new VertexNC(new Vector3(-u, -u, -u),   Color.Brown),
                    new VertexNC(new Vector3(u, -u, -u),    Color.Red),
                    new VertexNC(new Vector3(u, u, -u),     Color.Orange),
                    new VertexNC(new Vector3(-u, u, -u),    Color.Yellow),

                    new VertexNC(new Vector3(-u, -u, u),    Color.Green),
                    new VertexNC(new Vector3(u, -u, u),     Color.Cyan),
                    new VertexNC(new Vector3(u, u, u),      Color.Blue),
                    new VertexNC(new Vector3(-u, u, u),     Color.Magenta),
                },
                new[]
                {
                    new IndexTriangle(0, 3, 2),
                    new IndexTriangle(0, 2, 1),
                    new IndexTriangle(0, 1, 5),
                    new IndexTriangle(0, 5, 4),
                    new IndexTriangle(0, 4, 7),
                    new IndexTriangle(0, 7, 3),
                    new IndexTriangle(6, 5, 1),
                    new IndexTriangle(6, 1, 2),
                    new IndexTriangle(6, 2, 3),
                    new IndexTriangle(6, 3, 7),
                    new IndexTriangle(6, 7, 4),
                    new IndexTriangle(6, 4, 5),
                });
        }

        /// <summary>
        /// Returns a simple axis-aligned cube centered around (0, 0, 0).
        /// All edges are length 1.
        /// </summary>
        public static Mesh<VertexNC> CreateDirectionThing()
        {
            const float l = 1.0f;
            const float u = 0.05f;

            return new Mesh<VertexNC>(
                new[]
                {
                    new VertexNC(new Vector3(-u, -u, -u),   Color.Red),
                    new VertexNC(new Vector3(l + u, -u, -u),Color.Red),
                    new VertexNC(new Vector3(l + u, u, -u), Color.Red),
                    new VertexNC(new Vector3(-u, u, -u),    Color.Red),

                    new VertexNC(new Vector3(-u, -u, u),    Color.Red),
                    new VertexNC(new Vector3(l + u, -u, u), Color.Red),
                    new VertexNC(new Vector3(l + u, u, u),  Color.Red),
                    new VertexNC(new Vector3(-u, u, u),     Color.Red),

                    new VertexNC(new Vector3(-u, -u, -u),   Color.Green),
                    new VertexNC(new Vector3(u, -u, -u),    Color.Green),
                    new VertexNC(new Vector3(u, l + u, -u), Color.Green),
                    new VertexNC(new Vector3(-u, l + u, -u),Color.Green),

                    new VertexNC(new Vector3(-u, -u, u),    Color.Green),
                    new VertexNC(new Vector3(u, -u, u),     Color.Green),
                    new VertexNC(new Vector3(u, l + u, u),  Color.Green),
                    new VertexNC(new Vector3(-u, l + u, u), Color.Green),

                    new VertexNC(new Vector3(-u, -u, -u),   Color.Blue),
                    new VertexNC(new Vector3(u, -u, -u),    Color.Blue),
                    new VertexNC(new Vector3(u, u, -u),     Color.Blue),
                    new VertexNC(new Vector3(-u, u, -u),    Color.Blue),

                    new VertexNC(new Vector3(-u, -u, l + u),Color.Blue),
                    new VertexNC(new Vector3(u, -u, l + u), Color.Blue),
                    new VertexNC(new Vector3(u, u, l + u),  Color.Blue),
                    new VertexNC(new Vector3(-u, u, l + u), Color.Blue),
                },
                new[]
                {
                    new IndexTriangle(0, 3, 2),
                    new IndexTriangle(0, 2, 1),
                    new IndexTriangle(0, 1, 5),
                    new IndexTriangle(0, 5, 4),
                    new IndexTriangle(0, 4, 7),
                    new IndexTriangle(0, 7, 3),
                    new IndexTriangle(6, 5, 1),
                    new IndexTriangle(6, 1, 2),
                    new IndexTriangle(6, 2, 3),
                    new IndexTriangle(6, 3, 7),
                    new IndexTriangle(6, 7, 4),
                    new IndexTriangle(6, 4, 5),

                    new IndexTriangle(8 + 0, 8 + 3, 8 + 2),
                    new IndexTriangle(8 + 0, 8 + 2, 8 + 1),
                    new IndexTriangle(8 + 0, 8 + 1, 8 + 5),
                    new IndexTriangle(8 + 0, 8 + 5, 8 + 4),
                    new IndexTriangle(8 + 0, 8 + 4, 8 + 7),
                    new IndexTriangle(8 + 0, 8 + 7, 8 + 3),
                    new IndexTriangle(8 + 6, 8 + 5, 8 + 1),
                    new IndexTriangle(8 + 6, 8 + 1, 8 + 2),
                    new IndexTriangle(8 + 6, 8 + 2, 8 + 3),
                    new IndexTriangle(8 + 6, 8 + 3, 8 + 7),
                    new IndexTriangle(8 + 6, 8 + 7, 8 + 4),
                    new IndexTriangle(8 + 6, 8 + 4, 8 + 5),

                    new IndexTriangle(16 + 0, 16 + 3, 16 + 2),
                    new IndexTriangle(16 + 0, 16 + 2, 16 + 1),
                    new IndexTriangle(16 + 0, 16 + 1, 16 + 5),
                    new IndexTriangle(16 + 0, 16 + 5, 16 + 4),
                    new IndexTriangle(16 + 0, 16 + 4, 16 + 7),
                    new IndexTriangle(16 + 0, 16 + 7, 16 + 3),
                    new IndexTriangle(16 + 6, 16 + 5, 16 + 1),
                    new IndexTriangle(16 + 6, 16 + 1, 16 + 2),
                    new IndexTriangle(16 + 6, 16 + 2, 16 + 3),
                    new IndexTriangle(16 + 6, 16 + 3, 16 + 7),
                    new IndexTriangle(16 + 6, 16 + 7, 16 + 4),
                    new IndexTriangle(16 + 6, 16 + 4, 16 + 5),
                });
        }
    }
}