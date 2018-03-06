using System;
using OpenTK;
using SoSmooth.Rendering;

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
        
        private Vertex[] m_vertices;
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
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_name;
            }
        }

        /// <summary>
        /// The vertices of the mesh. The getter returns a copy of the
        /// actual array, so the setter must be used to update the 
        /// mesh.
        /// </summary>
        public Vertex[] Vertices
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_vertices.Clone() as Vertex[];
            }
            set
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                if (m_vertices.Length != value.Length)
                {
                    Array.Resize(ref m_vertices, value.Length);
                }
                Array.Copy(value, m_vertices, value.Length);
                m_vertexBufferDirty = true;
                RecalculateBounds();
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
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                return m_triangles.Clone() as Triangle[];
            }
            set
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
                if (m_triangles.Length != value.Length)
                {
                    Array.Resize(ref m_triangles, value.Length);
                }
                Array.Copy(value, m_triangles, value.Length);
                m_indexBufferDirty = true;
            }
        }

        /// <summary>
        /// The bounding box of the mesh.
        /// </summary>
        public Bounds Bounds
        {
            get
            {
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }
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
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

                if (m_vertexBufferDirty)
                {
                    UpdateVertices(Vertex.ToVertexPNC);
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
                if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

                if (m_indexBufferDirty)
                {
                    UpdateIndices();
                }
                return m_indexBuffer;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="Mesh"/>.
        /// </summary>
        /// <param name="name">The name of the mesh.</param>
        /// <param name="vertices">A vertex array.</param>
        /// <param name="triangles">A triangle array.</param>
        public Mesh(
            string name,
            Vertex[] vertices, 
            Triangle[] triangles)
        {
            m_name = name;

            m_vertices = vertices;
            m_triangles = triangles;
            
            m_vertexBufferDirty = true;
            m_indexBufferDirty = true;

            RecalculateBounds();
        }

        /// <summary>
        /// Clones a <see cref="Mesh"/>.
        /// </summary>
        /// <param name="mesh">A mesh to clone.</param>
        public Mesh(Mesh mesh)
        {
            m_name = mesh.Name;

            m_vertices = mesh.m_vertices.Clone() as Vertex[];
            m_triangles = mesh.m_triangles.Clone() as Triangle[];

            m_bounds = mesh.m_bounds;
            
            m_vertexBufferDirty = true;
            m_indexBufferDirty = true;
        }

        /// <summary>
        /// Computes vertex normals for the mesh.
        /// </summary>
        public void RecalculateNormals()
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            // clear the existing vertex normals
            for (int i = 0; i < m_vertices.Length; i++)
            {
                m_vertices[i].normal = Vector3.Zero;
            }

            // add the normal of every triangle to its vertices
            for (int i = 0; i < m_triangles.Length; i++)
            {
                Vector3 p0 = m_vertices[m_triangles[i].index0].position;
                Vector3 p1 = m_vertices[m_triangles[i].index1].position;
                Vector3 p2 = m_vertices[m_triangles[i].index2].position;

                Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);

                m_vertices[m_triangles[i].index0].normal += normal;
                m_vertices[m_triangles[i].index1].normal += normal;
                m_vertices[m_triangles[i].index2].normal += normal;
            }

            // normalize the result
            for (int i = 0; i < m_vertices.Length; i++)
            {
                m_vertices[i].normal.Normalize();
            }

            m_vertexBufferDirty = true;
        }

        /// <summary>
        /// Computes the axis-aligned bounding box of the mesh.
        /// </summary>
        private void RecalculateBounds()
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            
            for (int i = 0; i < m_vertices.Length; i++)
            {
                Vector3 pos = m_vertices[i].position;

                if (pos.X < min.X) { min.X = pos.X; }
                if (pos.Y < min.Y) { min.Y = pos.Y; }
                if (pos.Z < min.Z) { min.Z = pos.Z; }

                if (pos.X > max.X) { max.X = pos.X; }
                if (pos.Y > max.Y) { max.Y = pos.Y; }
                if (pos.Z > max.Z) { max.Z = pos.Z; }
            }

            m_bounds = Bounds.FromCorners(min, max);
        }

        /// <summary>
        /// Updates the vertex buffer object from the <see cref="Vertices"/> array.
        /// </summary>
        /// <typeparam name="TVertex">The type of vertex buffer to use.</typeparam>
        /// <param name="transform">Function that takes outputs vertices in a bufferable format.</param>
        private void UpdateVertices<TVertex>(Func<Vertex, TVertex> transform) where TVertex : struct, IVertexData
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
                vertexArray[i] = transform(m_vertices[i]);
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
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

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