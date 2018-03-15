using OpenTK;

namespace SoSmooth
{
    /// <summary>
    /// Represents a plane in 3d space.
    /// </summary>
    public struct Plane
    {
        private Vector4 m_plane;
        
        /// <summary>
        /// Creates a plane.
        /// </summary>
        /// <param name="plane">A plane given by a normal (xyz) and distance (w).</param>
        public Plane(Vector4 plane)
        {
            m_plane = plane;
            m_plane /= plane.Xyz.Length;
        }

        /// <summary>
        /// Creates a plane from a normal and a point on the plane.
        /// </summary>
        /// <param name="normal">The normal of the plane.</param>
        /// <param name="point">A point on the plane.</param>
        public Plane(Vector3 normal, Vector3 point)
        {
            normal.Normalize();
            m_plane = new Vector4(normal, -Vector3.Dot(normal, point));
        }

        /// <summary>
        /// Creates a plane from three points.
        /// </summary>
        /// <param name="p0">The first point.</param>
        /// <param name="p1">The second point.</param>
        /// <param name="p2">The third point.</param>
        public Plane(Vector3 p0, Vector3 p1, Vector3 p2) : this(Vector3.Cross(p1 - p0, p2 - p0), p0) { }

        /// <summary>
        /// Gets the intersection of this plane and a line.
        /// </summary>
        /// <param name="linePoint">A point on the line.</param>
        /// <param name="lineDirection">The direction of the line.</param>
        /// <param name="intersect">The line-plane intersect. Has default value if no intersect.</param>
        /// <returns>True if the line intersects the plane.</returns>
        public bool RaycastPlane(Vector3 linePoint, Vector3 lineDirection, out Vector3 intersect)
        {
            intersect = default(Vector3);

            Vector3 normal = m_plane.Xyz;
            lineDirection.Normalize();

            float nDotDir = Vector3.Dot(normal, lineDirection);
            if (nDotDir != 0)
            {
                float nDotPoint = Vector3.Dot(normal, linePoint);
                intersect = ((-(m_plane.W + nDotPoint) / nDotDir) * lineDirection) + linePoint;
                return true;
            }
            return false;
        }

        public static implicit operator Vector4(Plane p)
        {
            return p.m_plane;
        }
    }
}
