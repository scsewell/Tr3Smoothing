using System;
using OpenTK;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Represents an axis aligned bounding box (AABB).
    /// </summary>
    public struct Bounds
    {
        /// <summary>
        /// The center of the box.
        /// </summary>
        public readonly Vector3 Center;

        /// <summary>
        /// The length of each side of the box.
        /// </summary>
        public readonly Vector3 Size;

        /// <summary>
        /// The corners of the bounding box.
        /// </summary>
        public readonly Vector3[] Corners;

        /// <summary>
        /// Creates a new <see cref="Bounds"/> instance.
        /// </summary>
        /// <param name="center">The center of the bounding box.</param>
        /// <param name="size">The length of each size of the box.</param>
        public Bounds(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = new Vector3(Math.Abs(size.X), Math.Abs(size.Y), Math.Abs(size.Z));
            
            Vector3 half = 0.5f * Size;
            Corners = new Vector3[]
            {
                Center + new Vector3( half.X,  half.Y,  half.Z),
                Center + new Vector3( half.X,  half.Y, -half.Z),
                Center + new Vector3( half.X, -half.Y,  half.Z),
                Center + new Vector3( half.X, -half.Y, -half.Z),
                Center + new Vector3(-half.X,  half.Y,  half.Z),
                Center + new Vector3(-half.X,  half.Y, -half.Z),
                Center + new Vector3(-half.X, -half.Y,  half.Z),
                Center + new Vector3(-half.X, -half.Y, -half.Z),
            };
        }

        /// <summary>
        /// Creates a new <see cref="Bounds"/> instance from two opposite corners.
        /// </summary>
        /// <param name="minCorner">A corner on the bounding box.</param>
        /// <param name="maxCorner">The corner opposite of the minCorner on the box.</param>
        /// <returns>A new bounds instance.</returns>
        public static Bounds FromCorners(Vector3 minCorner, Vector3 maxCorner)
        {
            Vector3 center = 0.5f * (minCorner + maxCorner);
            Vector3 delta = maxCorner - minCorner;

            // ensure that the size uses positive values
            Vector3 size = new Vector3(
                Math.Abs(delta.X),
                Math.Abs(delta.Y),
                Math.Abs(delta.Z)
            );
            return new Bounds(center, size);
        }

        /// <summary>
        /// Gets an AABB that will bound all corners of this bounds after applying some transformation.
        /// </summary>
        /// <param name="transform">The transformation to apply to these bounds.</param>
        /// <returns>A new bounds instance.</returns>
        public Bounds Transformed(Matrix4 transform)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (Vector3 corner in Corners)
            {
                Vector3 pos = Vector3.TransformPosition(corner, transform);
                if (pos.X < min.X) { min.X = pos.X; }
                if (pos.Y < min.Y) { min.Y = pos.Y; }
                if (pos.Z < min.Z) { min.Z = pos.Z; }

                if (pos.X > max.X) { max.X = pos.X; }
                if (pos.Y > max.Y) { max.Y = pos.Y; }
                if (pos.Z > max.Z) { max.Z = pos.Z; }
            }
            return FromCorners(min, max);
        }

        /// <summary>
        /// Tests if this bounding box overlaps with or is contained by a frustum.
        /// </summary>
        public bool InFrustum(Vector4[] planes)
        {
            for (int i = 0; i < 6; i++)
            {
                int outside = 0;
                foreach (Vector3 corner in Corners)
                {
                    if (Vector4.Dot(planes[i], new Vector4(corner, 1.0f)) < 0)
                    {
                        outside++;
                    }
                }
                // if all points lay on the far side of a frustum plane the box can't be in the frustum
                if (outside == Corners.Length)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
