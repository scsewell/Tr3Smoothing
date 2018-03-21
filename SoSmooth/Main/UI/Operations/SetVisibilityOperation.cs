using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoSmooth.Meshes;

namespace SoSmooth
{
    public class SetVisibilityOperation : Operation
    {
        private readonly List<Mesh> m_meshes = new List<Mesh>();

        ///// <summary>
        ///// Creates a new <see cref="SetVisibilityOperation"/> instance.
        ///// </summary>
        ///// <param name="meshes">The meshes to set the visibility of.</param>
        //public SetVisibilityOperation(params Mesh[] meshes) 
        //    : this(meshes as IEnumerable<Mesh>)
        //{
        //}

        ///// <summary>
        ///// Creates a new <see cref="SmoothOperation"/> instance.
        ///// </summary>
        ///// <param name="meshes">The meshes to smooth.</param>
        //public SmoothOperation(ISmoother smoother, IEnumerable<Mesh> meshes)
        //{
        //    foreach (Mesh mesh in meshes)
        //    {
        //        m_meshes.Add(mesh);

        //        Triangle[] tris = mesh.Triangles;
        //        Vector3[] oldVerts = mesh.Vertices;
        //        Vector3[] oldNormals = mesh.Normals;

        //        Vector3[] newVerts = smoother.Smooth(oldVerts, tris);
        //        Vector3[] newNormals = Utils.CalculateNormals(newVerts, tris);

        //        m_oldVerts.Add(oldVerts);
        //        m_newVerts.Add(newVerts);
        //        m_oldNormals.Add(oldNormals);
        //        m_newNormals.Add(newNormals);
        //    }

        //    Excecute();
        //}

        /// <summary>
        /// Excecutes the operation.
        /// </summary>
        public override void Excecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
            }
        }

        /// <summary>
        /// Unexecutes the operation.
        /// </summary>
        public override void Unexcecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
            }
        }
    }
}
