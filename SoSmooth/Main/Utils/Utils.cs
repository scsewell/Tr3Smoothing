using System;
using OpenTK;

namespace SoSmooth
{
    /// <summary>
    /// A class that contains generally useful methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="from">The value at t=0.</param>
        /// <param name="to">The value at t=1.</param>
        /// <param name="t">Parameter clamped to the range [0, 1].</param>
        /// <returns>A value interpolated between from and to by factor t.</returns>
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * MathHelper.Clamp(t, 0, 1);
        }
        
        /// <summary>
        /// Smoothly eases a value using a Cosine fuction.
        /// </summary>
        /// <param name="t">A value in the range [0, 1].</param>
        /// <returns>An eased value in the range [0, 1]</returns>
        public static float Ease(float t)
        {
            return -0.5f * (float)Math.Cos(MathHelper.Clamp(t, 0, 1) * Math.PI) + 0.5f;
        }
        
        /// <summary>
        /// Calculates the barycentric coordinates of a points for a triangle.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="p0">The triangle's first vertex.</param>
        /// <param name="p1">The triangle's second vertex.</param>
        /// <param name="p2">The triangle's third vertex.</param>
        /// <returns>The barycentric coordinates of the point.</returns>
        public static Vector3 ToBarycentricCoords(Vector3 point, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 v0 = p1 - p0;
            Vector3 v1 = p2 - p0;
            Vector3 v2 = point - p0;
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;

            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }
        
        /// <summary>
        /// Gets the intersection of a line and triangle if it exists.
        /// </summary>
        /// <param name="p0">The first triangle vertex.</param>
        /// <param name="p1">The second triangle vertex.</param>
        /// <param name="p2">The third triangle vertex.</param>
        /// <param name="linePoint">A point on the line.</param>
        /// <param name="LineDirection">The direction of the line.</param>
        /// <param name="intersect">The intersect on the triangle's plane. Zero if no intersect.</param>
        /// <returns>True if the line intersects the triangle.</returns>
        public static bool RaycastTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 linePoint, Vector3 lineDirection, out Vector3 intersect)
        {
            Plane p = new Plane(p0, p1, p2);
            if (p.RaycastPlane(linePoint, lineDirection, out intersect))
            {
                Vector3 b = ToBarycentricCoords(intersect, p0, p1, p2);
                return 
                    0 <= b.X && b.X <= 1 &&
                    0 <= b.Y && b.Y <= 1 &&
                    0 <= b.Z && b.Z <= 1;
            }
            return false;
        }

        /// <summary>
        /// Calculates the point on a line closest to another point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="lineDirection">The direction of the line.</param>
        /// <param name="linePoint">The point the line passes through.</param>
        /// <returns>The closest point on the line.</returns>
        public static Vector3 ClosestPointOnLine(Vector3 point, Vector3 lineDirection, Vector3 linePoint)
        {
            lineDirection.Normalize();
            Vector3 p = point - linePoint;
            float d = Vector3.Dot(p, lineDirection);
            return d * lineDirection + linePoint;
        }

        /// <summary>
        /// Calculates the distance from a line to a point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="lineDirection">The direction of the line.</param>
        /// <param name="linePoint">The point the line passes through.</param>
        /// <returns>The minimal distance from the point to the line.</returns>
        public static float DistanceFromLine(Vector3 point, Vector3 lineDirection, Vector3 linePoint)
        {
            Vector3 p = point - linePoint;
            float d = Vector3.Dot(lineDirection.Normalized(), p);
            return (float)Math.Sqrt(Vector3.Dot(p, p) - (d * d));
        }

        /// <summary>
        /// Converts a rotation to euler angles.
        /// </summary>
        /// <param name="rot">The rotation to convert to euler angles.</param>
        /// <returns>A vector containing the pitch, roll, and yaw.</returns>
        public static Vector3 ToEulerAngles(this Quaternion rot)
        {
            Vector3 euler;

            // pitch (x-axis rotation)
            float sinr = 2.0f * (rot.W * rot.X + rot.Y * rot.Z);
            float cosr = 1.0f - 2.0f * (rot.X * rot.X + rot.Y * rot.Y);
            euler.X = (float)Math.Atan2(sinr, cosr);

            // roll (y-axis rotation)
            float sinp = 2.0f * (rot.W * rot.Y - rot.Z * rot.X);
            if (Math.Abs(sinp) >= 1)
            {
                sinp = Math.Sign(sinp);
            }
            euler.Y = (float)Math.Asin(sinp);

            // yaw (z-axis rotation)
            float siny = 2.0f * (rot.W * rot.Z + rot.X * rot.Y);
            float cosy = 1.0f - 2.0f * (rot.Y * rot.Y + rot.Z * rot.Z);
            euler.Z = (float)Math.Atan2(siny, cosy);

            return euler;
        }

        /// <summary>
        /// Creates a transformation matrix from scalars.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="result">A matrix instance.</param>
        public static void CreateTransform(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, out Matrix4 result)
        {
            result = Matrix4.Identity;
            
            // get the axis angle rotation from the quaternion
            if (Math.Abs(rotation.W) > 1.0f)
            {
                rotation.Normalize();
            }

            Vector3 axis;
            float angle = 2.0f * (float)Math.Acos(rotation.W);
            float den = (float)Math.Sqrt(1.0 - rotation.W * rotation.W);
            if (den > 0.001f)
            {
                axis = rotation.Xyz / den;
                axis.Normalize();
            }
            else
            {
                axis = Vector3.UnitX;
            }
            
            // calculate angles
            float cos = (float)Math.Cos(-angle);
            float sin = (float)Math.Sin(-angle);
            float t = 1.0f - cos;

            // do the conversion math once
            float tXX = t * axis.X * axis.X;
            float tXY = t * axis.X * axis.Y;
            float tXZ = t * axis.X * axis.Z;
            float tYY = t * axis.Y * axis.Y;
            float tYZ = t * axis.Y * axis.Z;
            float tZZ = t * axis.Z * axis.Z;

            float sinX = sin * axis.X;
            float sinY = sin * axis.Y;
            float sinZ = sin * axis.Z;

            result.Row0.X = scale.X * (tXX + cos);
            result.Row0.Y = scale.X * (tXY - sinZ);
            result.Row0.Z = scale.X * (tXZ + sinY);
            result.Row1.X = scale.Y * (tXY + sinZ);
            result.Row1.Y = scale.Y * (tYY + cos);
            result.Row1.Z = scale.Y * (tYZ - sinX);
            result.Row2.X = scale.Z * (tXZ - sinY);
            result.Row2.Y = scale.Z * (tYZ + sinX);
            result.Row2.Z = scale.Z * (tZZ + cos);

            // set the position
            result.Row3.X = position.X;
            result.Row3.Y = position.Y;
            result.Row3.Z = position.Z;
        }
    }
}
