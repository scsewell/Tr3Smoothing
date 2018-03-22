using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Performs smoothing operations.
    /// </summary>
    public class SmoothOperation : Operation
    {
        private readonly List<MeshInfo> m_meshes = new List<MeshInfo>();
        private readonly List<Vector3[]> m_oldVerts = new List<Vector3[]>();
        private readonly List<Vector3[]> m_oldNormals = new List<Vector3[]>();
        private readonly List<Vector3[]> m_newVerts = new List<Vector3[]>();
        private readonly List<Vector3[]> m_newNormals = new List<Vector3[]>();

        /// <summary>
        /// Creates a new <see cref="SmoothOperation"/> instance.
        /// </summary>
        /// <param name="smoother">The smoothing algorithm to apply.</param>
        /// <param name="meshes">The meshes to smooth.</param>
        public SmoothOperation(ISmoother smoother, params MeshInfo[] meshes) 
            : this(smoother, meshes as IEnumerable<MeshInfo>)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SmoothOperation"/> instance.
        /// </summary>
        /// <param name="smoother">The smoothing algorithm to apply.</param>
        /// <param name="meshes">The meshes to smooth.</param>
        public SmoothOperation(ISmoother smoother, IEnumerable<MeshInfo> meshes)
        {
            foreach (MeshInfo info in meshes)
            {
                m_meshes.Add(info);

                Mesh mesh = info.Mesh;
                Triangle[] tris = mesh.Triangles;
                Vector3[] oldVerts = mesh.Vertices;
                Vector3[] oldNormals = mesh.Normals;

                // We store the before and after results of the smoothing to make undoing
                // and redoing very fast rather than recalculating the smoothing.
                Vector3[] newVerts = smoother.Smooth(oldVerts, tris);
                Vector3[] newNormals = Utils.CalculateNormals(newVerts, tris);

                m_oldVerts.Add(oldVerts);
                m_newVerts.Add(newVerts);
                m_oldNormals.Add(oldNormals);
                m_newNormals.Add(newNormals);
            }

            Excecute();
            UndoStack.Instance.AddOperation(this);
        }

        /// <summary>
        /// Excecutes the operation.
        /// </summary>
        public override void Excecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
                Mesh mesh = m_meshes[i].Mesh;
                mesh.Vertices = m_newVerts[i];
                mesh.Normals = m_newNormals[i];
            }
        }

        /// <summary>
        /// Unexecutes the operation.
        /// </summary>
        public override void Unexcecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
                Mesh mesh = m_meshes[i].Mesh;
                mesh.Vertices = m_oldVerts[i];
                mesh.Normals = m_oldNormals[i];
            }
        }
    }
}
