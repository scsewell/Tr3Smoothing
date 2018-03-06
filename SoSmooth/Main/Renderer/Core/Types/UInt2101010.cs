using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Packs a three dimentional unsigned vector using three 10-bit components. 
    /// This often used in graphics to represent HDR colors.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UInt2101010
    {
        private uint m_value;
        
        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="c">The color to pack. Each component should be in the range [0, 1].</param>
        public UInt2101010(Color4 c) : this(c.R, c.G, c.B, c.A) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="v">The vector to pack. Each component should be in the range [0, 1].</param>
        public UInt2101010(Vector3h v) : this(v.X, v.Y, v.Z) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="v">The vector to pack. Each component should be in the range [0, 1].</param>
        public UInt2101010(Vector3 v) : this(v.X, v.Y, v.Z) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="v">The vector to pack. Each component should be in the range [0, 1].</param>
        public UInt2101010(Vector3d v) : this((float)v.X, (float)v.Y, (float)v.Z) { }

        /// <summary>
        /// Packs a vector as a <see cref="Int2101010"/>.
        /// </summary>
        /// <param name="x">The first component [0, 1] (10 bits).</param>
        /// <param name="y">The second component [0, 1] (10 bits).</param>
        /// <param name="z">The third component [0, 1] (10 bits).</param>
        /// <param name="w">The fourth component [0, 1] (2 bits).</param>
        public UInt2101010(float x, float y, float z, float w = 0)
        {
            m_value =  ((uint)(x * ((1 << 10) - 1)) & 0x00003FF);
            m_value |= ((uint)(y * ((1 << 10) - 1)) & 0x00003FF) << 10;
            m_value |= ((uint)(z * ((1 << 10) - 1)) & 0x00003FF) << 20;
            m_value |= ((uint)(w * ((1 <<  2) - 1)) & 0x0000003) << 30;
        }
    }
}
