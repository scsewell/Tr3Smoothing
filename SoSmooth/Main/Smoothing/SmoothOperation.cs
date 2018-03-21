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
        private readonly List<Mesh> m_meshes = new List<Mesh>();
        private readonly List<Vector3[]> m_oldVerts = new List<Vector3[]>();
        private readonly List<Vector3[]> m_oldNormals = new List<Vector3[]>();
        private readonly List<Vector3[]> m_newVerts = new List<Vector3[]>();
        private readonly List<Vector3[]> m_newNormals = new List<Vector3[]>();

        /// <summary>
        /// Creates a new <see cref="SmoothOperation"/> instance.
        /// </summary>
        /// <param name="smoother">The smoothing algorithm to apply.</param>
        /// <param name="meshes">The meshes to smooth.</param>
        public SmoothOperation(ISmoother smoother, params Mesh[] meshes) 
            : this(smoother, meshes as IEnumerable<Mesh>)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SmoothOperation"/> instance.
        /// </summary>
        /// <param name="smoother">The smoothing algorithm to apply.</param>
        /// <param name="meshes">The meshes to smooth.</param>
        public SmoothOperation(ISmoother smoother, IEnumerable<Mesh> meshes)
        {
            foreach (Mesh mesh in meshes)
            {
                m_meshes.Add(mesh);

                Triangle[] tris = mesh.Triangles;
                Vector3[] oldVerts = mesh.Vertices;
                Vector3[] oldNormals = mesh.Normals;

                Vector3[] newVerts = smoother.Smooth(oldVerts, tris);
                Vector3[] newNormals = Utils.CalculateNormals(newVerts, tris);

                m_oldVerts.Add(oldVerts);
                m_newVerts.Add(newVerts);
                m_oldNormals.Add(oldNormals);
                m_newNormals.Add(newNormals);
            }

            Excecute();
        }

        /// <summary>
        /// Excecutes the operation.
        /// </summary>
        public override void Excecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
                m_meshes[i].Vertices = m_newVerts[i];
                m_meshes[i].Normals = m_newNormals[i];
            }
        }

        /// <summary>
        /// Unexecutes the operation.
        /// </summary>
        public override void Unexcecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
                m_meshes[i].Vertices = m_oldVerts[i];
                m_meshes[i].Normals = m_oldNormals[i];
            }
        }
    }
}
