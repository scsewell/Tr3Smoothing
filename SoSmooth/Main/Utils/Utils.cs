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
        /// Normalizes a plane.
        /// </summary>
        /// <param name="plane">The plane to normalize.</param>
        /// <returns>The plane with a normalized normal.</returns>
        public static Vector4 NormalizePlane(Vector4 plane)
        {
            plane /= new Vector3(plane).Length;
            return plane;
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
