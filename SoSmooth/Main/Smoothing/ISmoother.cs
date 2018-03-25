using OpenTK;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Interface for smoothing algorithms.
    /// </summary>
    public interface ISmoother
    {
        /// <summary>
        /// Smooths a mesh.
        /// </summary>
        /// <param name="verts">The vertices of the mesh.</param>
        /// <param name="tris">The triangles of the mesh.</param>
        /// <returns>The smoothed vertex positions.</returns>
        Vector3[] Smooth(Vector3[] verts, Triangle[] tris);

        /// <summary>
        /// Checks if the current settings will cause the smoother to have no effect.
        /// </summary>
        bool WillNoOp();
    }
}
