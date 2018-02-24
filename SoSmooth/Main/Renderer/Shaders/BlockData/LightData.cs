using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Stores lighting information in a struct that can be passed into a GLSL interface block.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct LightData : IEquatable<LightData>
    {
        [FieldOffset(0 * sizeof(float))]
        public Color4 AmbientColor;

        [FieldOffset(4 * sizeof(float))]
        public Vector3 Direction0;
        [FieldOffset(8 * sizeof(float))]
        public Vector3 Direction1;
        [FieldOffset(12 * sizeof(float))]
        public Vector3 Direction2;

        [FieldOffset(16 * sizeof(float))]
        public Color4 DiffColor0;
        [FieldOffset(20 * sizeof(float))]
        public Color4 DiffColor1;
        [FieldOffset(24 * sizeof(float))]
        public Color4 DiffColor2;

        [FieldOffset(28 * sizeof(float))]
        public Color4 SpecColor0;
        [FieldOffset(32 * sizeof(float))]
        public Color4 SpecColor1;
        [FieldOffset(36 * sizeof(float))]
        public Color4 SpecColor2;
        
        /// <summary>
        /// Checks if the stored values are equivalent to another set of values.
        /// </summary>
        /// <param name="other">The values to compare with.</param>
        /// <returns>True if equivalent.</returns>
        public bool Equals(LightData other)
        {
            return
                AmbientColor == other.AmbientColor &&

                Direction0 == other.Direction0 &&
                Direction1 == other.Direction1 &&
                Direction2 == other.Direction2 &&

                DiffColor0 == other.DiffColor0 &&
                DiffColor1 == other.DiffColor1 &&
                DiffColor2 == other.DiffColor2 &&

                SpecColor0 == other.SpecColor0 &&
                SpecColor1 == other.SpecColor1 &&
                SpecColor2 == other.SpecColor2;
        }
    }
}
