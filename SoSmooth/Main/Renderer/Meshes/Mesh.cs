using System;
using OpenTK;
using OpenTK.Graphics;
using SoSmooth.Rendering;
using SoSmooth.Rendering.Vertices;

namespace SoSmooth.Meshes
{
    /// <summary>
    /// Represents a mesh of vertices and triangles. Manages uploading the 
    /// vertex buffer object and index buffer that represent the mesh on
    /// the GPU.
    /// </summary>
    public sealed class Mesh : Disposable
    {
        private string m_name;
        
        private Vector3[] m_vertices;
        private Vector3[] m_normals;
        private Color4[] m_colors;
        private Triangle[] m_triangles;
        private Bounds m_bounds;

        private IVertexBuffer m_vertexBuffer;
        private bool m_vertexBufferDirty;

        private IIndexBuffer m_indexBuffer;
        private bool m_indexBufferDirty;

        /// <summary>
        /// The name of this mesh.
        /// </summary>
        public string Name
        {
            get
            {
                ValidateDispose();
                return m_name;
            }
        }

        /// <summary>
        /// Gets the number of vertices in the mesh.
        /// </summary>
        public int VertexCount
        {
            get
            {
                ValidateDispose();
                return m_vertices.Length;
            }
        }

        /// <summary>
        /// The vertices of the mesh. The getter returns a copy of the actual array, so the setter
        /// must be used to update the mesh. When setting and the length has changed, all other
        /// vertex data arrays will be resized to the same length.
        /// </summary>
        public Vector3[] Vertices
        {
            get
            {
                ValidateDispose();
                return m_vertices.Clone() as Vector3[];
            }
            set
            {
                ValidateDispose();
                if (m_vertices.Length != value.Length)
                {
                    Array.Resize(ref m_vertices, value.Length);
                    Array.Resize(ref m_normals, value.Length);
                    Array.Resize(ref m_colors, value.Length);
                }
                Array.Copy(value, m_vertices, value.Length);
                m_vertexBufferDirty = true;

                // the bounds are no longer valid if the vertex positions were modified
                m_bounds = Bounds.FromPoints(m_vertices);
                MeshModified?.Invoke();
            }
        }

        /// <summary>
        /// The vertex normals of the mesh. The getter returns a copy of the actual array, so the setter
        /// must be used to update the mesh. When setting and the length has changed, all other
        /// vertex data arrays will be resized to the same length. When set to null, normals will be
        /// automatically generated.
        /// </summary>
        public Vector3[] Normals
        {
            get
            {
                ValidateDispose();
                return m_normals.Clone() as Vector3[];
            }
            set
            {
                ValidateDispose();
                if (value == null)
                {
                    RecalculateNormals();
                }
                else
                {
                    if (m_normals.Length != value.Length)
                    {
                        Array.Resize(ref m_vertices, value.Length);
                        Array.Resize(ref m_normals, value.Length);
                        Array.Resize(ref m_colors, value.Length);
                    }
                    Array.Copy(value, m_normals, value.Length);
                    m_vertexBufferDirty = true;
                    MeshModified?.Invoke();
                }
            }
        }

        /// <summary>
        /// The vertex colors of the mesh. The getter returns a copy of the actual array, so the setter
        /// must be used to update the mesh. When setting and the length has changed, all other
        /// vertex data arrays will be resized to the same length. When set to null, all vertex colors
        /// will be set to white.
        /// </summary>
        public Color4[] Colors
        {
            get
            {
                ValidateDispose();
                return m_colors.Clone() as Color4[];
            }
            set
            {
                ValidateDispose();
                if (value == null)
                {
                    for (int i = 0; i < m_colors.Length; i++)
                    {
                        m_colors[i] = Color4.White;
                    }
                }
                else
                {
                    if (m_colors.Length != value.Length)
                    {
                        Array.Resize(ref m_vertices, value.Length);
                        Array.Resize(ref m_normals, value.Length);
                        Array.Resize(ref m_colors, value.Length);
                    }
                    Array.Copy(value, m_colors, value.Length);
                }
                m_vertexBufferDirty = true;
                MeshModified?.Invoke();
            }
        }

        /// <summary>
        /// Gets the number of triangles in the mesh.
        /// </summary>
        public int TriangleCount
        {
            get
            {
                ValidateDispose();
                return m_triangles.Length;
            }
        }

        /// <summary>
        /// The triangles of the mesh. The getter returns a copy of the
        /// actual array, so the setter must be used to update the 
        /// mesh.
        /// </summary>
        public Triangle[] Triangles
        {
            get
            {
                ValidateDispose();
                return m_triangles.Clone() as Triangle[];
            }
            set
            {
                ValidateDispose();
                if (m_triangles.Length != value.Length)
                {
                    Array.Resize(ref m_triangles, value.Length);
                }
                Array.Copy(value, m_triangles, value.Length);
                m_indexBufferDirty = true;
                MeshModified?.Invoke();
            }
        }

        /// <summary>
        /// The bounding box of the mesh.
        /// </summary>
        public Bounds Bounds
        {
            get
            {
                ValidateDispose();
                return m_bounds;
            }
        }
        
        /// <summary>
        /// The vertex buffer object.
        /// </summary>
        public IVertexBuffer VBO
        {
            get
            {
                ValidateDispose();
                if (m_vertexBufferDirty)
                {
                    UpdateVertices((pos, nrm, col) => new VertexPNC(pos, nrm, col));
                }
                return m_vertexBuffer;
            }
        }

        /// <summary>
        /// The index buffer object.
        /// </summary>
        public IIndexBuffer IBO
        {
            get
            {
                ValidateDispose();
                if (m_indexBufferDirty)
                {
                    UpdateIndices();
                }
                return m_indexBuffer;
            }
        }

        /// <summary>
        /// Triggered when the mesh vertices or triangles have been changed.
        /// </summary>
        public event Action MeshModified;

        /// <summary>
        /// Constructs a new <see cref="Mesh"/> instance.
        /// </summary>
        /// <param name="name">The name of the mesh.</param>
        /// <param name="vertices">The vertex positions.</param>
        /// <param name="normals">The vertex normals. If null they will be automatically generated.</param>
        /// <param name="colors">The vertex colors. If null will default to white.</param>
        /// <param name="triangles">A triangle array.</param>
        public Mesh(
            string name,
            Vector3[] vertices,
            Vector3[] normals,
            Color4[] colors,
            Triangle[] triangles)
        {
            m_name = name;
            m_vertices = vertices;
            m_triangles = triangles;

            m_normals = normals;
            if (m_normals == null)
            {
                m_normals = Utils.CalculateNormals(vertices, triangles);
            }

            m_colors = colors;
            if (m_colors == null)
            {
                m_colors = new Color4[vertices.Length];
                for (int i = 0; i < m_colors.Length; i++)
                {
                    m_colors[i] = Color4.White;
                }
            }

            m_bounds = Bounds.FromPoints(m_vertices);

            m_vertexBufferDirty = true;
            m_indexBufferDirty = true;
        }

        /// <summary>
        /// Clones a <see cref="Mesh"/>.
        /// </summary>
        /// <param name="mesh">A mesh to clone.</param>
        public Mesh(Mesh mesh)
        {
            m_name = mesh.m_name;

            m_vertices  = mesh.m_vertices.Clone() as Vector3[];
            m_normals   = mesh.m_normals.Clone() as Vector3[];
            m_colors    = mesh.m_colors.Clone() as Color4[];
            m_triangles = mesh.m_triangles.Clone() as Triangle[];

            m_bounds = mesh.m_bounds;
            
            m_vertexBufferDirty = true;
            m_indexBufferDirty = true;
        }

        /// <summary>
        /// Computes vertex normals for the mesh.
        /// </summary>
        private void RecalculateNormals()
        {
            ValidateDispose();
            Utils.CalculateNormals(m_vertices, m_triangles, m_normals);
            m_vertexBufferDirty = true;
            MeshModified?.Invoke();
        }

        /// <summary>
        /// Updates the vertex buffer object from the <see cref="Vertices"/> array.
        /// </summary>
        /// <typeparam name="TVertex">The type of vertex buffer to use.</typeparam>
        /// <param name="transform">Function that transforms the vertex array data to a interleaved struct format.</param>
        private void UpdateVertices<TVertex>(
            Func<Vector3, Vector3, Color4, TVertex> transform
            ) where TVertex : struct, IVertexData
        {
            // if the type of the vertex data has changed we need to create a new buffer
            if (m_vertexBuffer != null && m_vertexBuffer.GetType() != typeof(VertexBuffer<TVertex>))
            {
                m_vertexBuffer.Dispose();
                m_vertexBuffer = null;
            }

            // create vertex buffer if needed
            if (m_vertexBuffer == null)
            {
                m_vertexBuffer = new VertexBuffer<TVertex>(m_vertices.Length);
            }

            // copy the vertices into the buffer
            VertexBuffer<TVertex> buffer = m_vertexBuffer as VertexBuffer<TVertex>;
            buffer.Clear();
            
            int offset;
            TVertex[] vertexArray = buffer.WriteDirectly(m_vertices.Length, out offset);
            
            for (int i = 0; i < m_vertices.Length; i++)
            {
                vertexArray[i] = transform(m_vertices[i], m_normals[i], m_colors[i]);
            }

            // upload to the GPU
            buffer.BufferData();
            m_vertexBufferDirty = false;
        }

        /// <summary>
        /// Updates the index buffer object from the <see cref="Triangles"/> array.
        /// Ensures that the smallest unsigned integer type possible is used for the buffer.
        /// </summary>
        private void UpdateIndices()
        {
            // if the current type of index buffer can't store enough vertices get rid of it
            if (m_indexBuffer != null && m_indexBuffer.MaxVertices < m_vertices.Length)
            {
                m_indexBuffer.Dispose();
                m_indexBuffer = null;
            }

            // create a new index buffer able to address all of the vertices if needed
            if (m_indexBuffer == null)
            {
                int capacity = m_triangles.Length * 3;

                if (m_vertices.Length <= byte.MaxValue)
                {
                    m_indexBuffer = new IndexBuffer<byte>(capacity);
                }
                else if (m_vertices.Length <= ushort.MaxValue)
                {
                    m_indexBuffer = new IndexBuffer<ushort>(capacity);
                }
                else
                {
                    m_indexBuffer = new IndexBuffer<uint>(capacity);
                }
            }

            // copy the triangles' indices into the buffer
            if (m_indexBuffer is IndexBuffer<byte>)
            {
                CopyIndices(m_indexBuffer as IndexBuffer<byte>, index => (byte)index);
            }
            else if (m_indexBuffer is IndexBuffer<ushort>)
            {
                CopyIndices(m_indexBuffer as IndexBuffer<ushort>, index => (ushort)index);
            }
            else
            {
                CopyIndices(m_indexBuffer as IndexBuffer<uint>, index => index);
            }

            // upload to the GPU
            m_indexBuffer.BufferData();
            m_indexBufferDirty = false;
        }

        /// <summary>
        /// Copies the triangle indices into an index buffer.
        /// </summary>
        /// <typeparam name="TIndex">The type of index to use.</typeparam>
        /// <param name="buffer">The index buffer to write indices into.</param>
        /// <param name="transform">Outputs the type of index needed in the buffer.</param>
        private void CopyIndices<TIndex>(IndexBuffer<TIndex> buffer, Func<uint, TIndex> transform) where TIndex : struct
        {
            buffer.Clear();

            int offset;
            TIndex[] indexArray = buffer.WriteDirectly(m_triangles.Length * 3, out offset);

            for (int i = 0; i < m_triangles.Length; i++)
            {
                Triangle triangle = m_triangles[i];

                indexArray[offset]      = transform(triangle.index0);
                indexArray[offset + 1]  = transform(triangle.index1);
                indexArray[offset + 2]  = transform(triangle.index2);

                offset += 3;
            }
        }

        /// <summary>
        /// Gets a string describing the instance.
        /// </summary>
        public override string ToString()
        {
            return $"{{name:\"{m_name}\" verts:{m_vertices.Length} tris:{m_triangles.Length}}}";
        }
        
        /// <summary>
        /// Cleanup of unmanaged resources.
        /// </summary>
        protected override void OnDispose(bool disposing)
        {
            if (m_vertexBuffer != null)
            {
                m_vertexBuffer.Dispose();
                m_vertexBuffer = null;
            }
            if (m_indexBuffer != null)
            {
                m_indexBuffer.Dispose();
                m_indexBuffer = null;
            }
        }
    }
}