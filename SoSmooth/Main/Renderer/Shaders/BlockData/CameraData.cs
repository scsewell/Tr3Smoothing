using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Stores camera information in a struct that can be passed into a GLSL interface block.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CameraData : IEquatable<CameraData>
    {
        public readonly Matrix4 ViewMat;
        public readonly Matrix4 ViewInvMat;
        public readonly Matrix4 ProjMat;
        public readonly Matrix4 ViewProjMat;
        public readonly Vector3 WorldPos;

        /// <summary>
        /// Constructs a new <see cref="CameraData"/> instance.
        /// </summary>
        /// <param name="viewMat">The camera's view matrix.</param>
        /// <param name="projMat">The camera's projection matrix.</param>
        public CameraData(Matrix4 viewMat, Matrix4 projMat)
        {
            ViewMat = viewMat;
            ProjMat = projMat;

            // store commonly used transformations of the matrices
            ViewInvMat = ViewMat.Inverted();
            ViewProjMat = ViewMat * ProjMat;

            // store the camera's world space position
            WorldPos = ViewInvMat.Row3.Xyz;
        }

        /// <summary>
        /// Checks if the stored values are equivalent to another set of values.
        /// </summary>
        /// <param name="other">The values to compare with.</param>
        /// <returns>True if equivalent.</returns>
        public bool Equals(CameraData other)
        {
            return 
                ViewMat == other.ViewMat &&
                ProjMat == other.ProjMat;
        }
    }
}
