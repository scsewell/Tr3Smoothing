using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// <param name="meshes">The meshes to smooth.</param>
        public SmoothOperation(params MeshInfo[] meshes) : this(meshes as IEnumerable<MeshInfo>)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SmoothOperation"/> instance.
        /// </summary>
        /// <param name="meshes">The meshes to smooth.</param>
        public SmoothOperation(IEnumerable<MeshInfo> meshes)
        {
            // Smooth each mesh on its own thread
            Dictionary<MeshInfo, Task<Vector3[]>> smoothingTasks = new Dictionary<MeshInfo, Task<Vector3[]>>();

            // We store the before and after results of the smoothing to make undoing
            // and redoing very fast rather than recalculating the smoothing.
            foreach (MeshInfo info in meshes)
            {
                m_meshes.Add(info);

                Mesh mesh = info.Mesh;
                Vector3[] oldVerts = mesh.Vertices;
                Vector3[] oldNormals = mesh.Normals;
                m_oldVerts.Add(oldVerts);
                m_oldNormals.Add(oldNormals);

                // start a thread for the smoothing of the mesh
                Task<Vector3[]> smoothTask = new Task<Vector3[]>(() => new MeanSmoother().Smooth(oldVerts, mesh.Triangles));
                smoothTask.Start();
                smoothingTasks.Add(info, smoothTask);
            }

            // wait for each smoothing thread and store results
            foreach (MeshInfo info in meshes)
            {
                Task<Vector3[]> task = smoothingTasks[info];
                task.Wait();
                Vector3[] newVerts = task.Result;
                Vector3[] newNormals = Utils.CalculateNormals(newVerts, info.Mesh.Triangles);
                m_newVerts.Add(newVerts);
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
            // apply the smoothed vertex positions and normals to the meshes
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
            // apply the original vertex positions and normals to the meshes
            for (int i = 0; i < m_meshes.Count; i++)
            {
                Mesh mesh = m_meshes[i].Mesh;
                mesh.Vertices = m_oldVerts[i];
                mesh.Normals = m_oldNormals[i];
            }
        }
    }
}
