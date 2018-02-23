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
        private Matrix4 m_viewMat;
        private Matrix4 m_viewInvMat;
        private Matrix4 m_projMat;
        private Matrix4 m_viewProjMat;
        private Vector3 m_worldPos;

        /// <summary>
        /// Constructs a new <see cref="CameraData"/> instance.
        /// </summary>
        /// <param name="viewMat">The camera's view matrix.</param>
        /// <param name="projMat">The camera's projection matrix.</param>
        public CameraData(Matrix4 viewMat, Matrix4 projMat)
        {
            m_viewMat = viewMat;
            m_projMat = projMat;

            // store commonly used transformations of the matrices
            m_viewInvMat = m_viewMat.Inverted();
            m_viewProjMat = m_viewMat * m_projMat;

            // store the camera's world space position
            m_worldPos = m_viewInvMat.Row3.Xyz;
        }

        /// <summary>
        /// Checks if the stored values are equivalent to another set of values.
        /// </summary>
        /// <param name="other">The values to compare with.</param>
        /// <returns>True if equivalent.</returns>
        public bool Equals(CameraData other)
        {
            return 
                m_viewMat == other.m_viewMat &&
                m_projMat == other.m_projMat;
        }
    }
}
