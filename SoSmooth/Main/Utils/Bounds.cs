using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenTK;

namespace SoSmooth
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
        public readonly ReadOnlyCollection<Vector3> Corners;

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
            Corners = Array.AsReadOnly(new Vector3[]
            {
                Center + new Vector3( half.X,  half.Y,  half.Z),
                Center + new Vector3( half.X,  half.Y, -half.Z),
                Center + new Vector3( half.X, -half.Y,  half.Z),
                Center + new Vector3( half.X, -half.Y, -half.Z),
                Center + new Vector3(-half.X,  half.Y,  half.Z),
                Center + new Vector3(-half.X,  half.Y, -half.Z),
                Center + new Vector3(-half.X, -half.Y,  half.Z),
                Center + new Vector3(-half.X, -half.Y, -half.Z),
            });
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
        /// Gets an AABB that will bound a set of transformation points.
        /// </summary>
        /// <param name="points">The set of points to bound.</param>
        /// <returns>A new axis aligned bounding box.</returns>
        public static Bounds FromPoints(IEnumerable<Vector3> points)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (Vector3 pos in points)
            {
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
        /// Gets an AABB that will bound a set of transformation points.
        /// </summary>
        /// <param name="points">The set of points to bound.</param>
        /// <param name="transform">The transformation to apply to all points.</param>
        /// <returns>A new axis aligned bounding box.</returns>
        public static Bounds FromPoints(IEnumerable<Vector3> points, Matrix4 transform)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (Vector3 point in points)
            {
                Vector3 pos = Vector3.TransformPosition(point, transform);
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
        /// Gets an AABB that will bound all corners of this bounds after applying some transformation.
        /// </summary>
        /// <param name="transform">The transformation to apply to these bounds.</param>
        /// <returns>A new bounds instance.</returns>
        public Bounds Transformed(Matrix4 transform)
        {
            return FromPoints(Corners, transform);
        }
        
        /// <summary>
        /// Tests if this bounding box overlaps with or is contained by a frustum.
        /// </summary>
        public bool InFrustum(Plane[] planes)
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
                if (outside == Corners.Count)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a ray intersects this bounds.
        /// </summary>
        /// <param name="origin">The ray's origin.</param>
        /// <param name="direction">The ray's direction.</param>
        /// <returns>True if the ray intersects the bounds.</returns>
        public bool Raycast(Vector3 origin, Vector3 direction)
        {
            Vector3 min = Center - (0.5f * Size);
            Vector3 max = Center + (0.5f * Size);

            float tmin = (min.X - origin.X) / direction.X;
            float tmax = (max.X - origin.X) / direction.X;
            float tymin = (min.Y - origin.Y) / direction.Y;
            float tymax = (max.Y - origin.Y) / direction.Y;

            if (tmin > tmax)
            {
                float temp = tmin;
                tmin = tmax;
                tmax = temp;
            }
            if (tymin > tymax)
            {
                float temp = tymin;
                tymin = tymax;
                tymax = temp;
            }

            if (tmin > tymax || tymin > tmax)
            {
                return false;
            }
            if (tymin > tmin)
            {
                tmin = tymin;
            }
            if (tymax < tmax)
            {
                tmax = tymax;
            }

            float tzmin = (min.Z - origin.Z) / direction.Z;
            float tzmax = (max.Z - origin.Z) / direction.Z;

            if (tzmin > tzmax)
            {
                float temp = tzmin;
                tzmin = tzmax;
                tzmax = temp;
            }

            if (tmin > tzmax || tzmin > tmax)
            {
                return false;
            }
            if (tzmin > tmin)
            {
                tmin = tzmin;
            }
            if (tzmax < tmax)
            {
                tmax = tzmax;
            }

            return true;
        }
    }
}
