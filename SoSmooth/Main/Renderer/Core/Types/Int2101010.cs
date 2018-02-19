using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Packs a three dimentional vector using three 10-bit components. This often used 
    /// in graphics to represent normals.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Int2101010
    {
        private int m_value;
        
        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="v">The vector to pack. The vector should be under unit length.</param>
        public Int2101010(Vector3h v) : this(v.X, v.Y, v.Z) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="v">The vector to pack. The vector should be under unit length.</param>
        public Int2101010(Vector3 v) : this(v.X, v.Y, v.Z) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="v">The vector to pack. The vector should be under unit length.</param>
        public Int2101010(Vector3d v) : this((float)v.X, (float)v.Y, (float)v.Z) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="x">The first component [-1, 1] (10 bits).</param>
        /// <param name="y">The second component [-1, 1] (10 bits).</param>
        /// <param name="z">The third component [-1, 1] (10 bits).</param>
        /// <param name="w">The fourth component [-1, 1] (2 bits).</param>
        public Int2101010(float x, float y, float z, float w = 0)
        {
            m_value  = ((int)(x * ((1 << 9) - 1)) & 0x00003FF);
            m_value |= ((int)(y * ((1 << 9) - 1)) & 0x00003FF) << 10;
            m_value |= ((int)(z * ((1 << 9) - 1)) & 0x00003FF) << 20;
            m_value |= ((int)(w * ((1 << 1) - 1)) & 0x0000003) << 30;
        }
    }
}
