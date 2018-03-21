using System.Runtime.InteropServices;
using OpenTK;

namespace SoSmooth.Meshes
{
    /// <summary>
    /// A triplet of vertex indices, representing a triangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Triangle
    {
        public readonly uint index0;
        public readonly uint index1;
        public readonly uint index2;
        
        /// <summary>
        /// Creates a new index triangle.
        /// </summary>
        public Triangle(uint i0, uint i1, uint i2)
        {
            index0 = i0;
            index1 = i1;
            index2 = i2;
        }

        /// <summary>
        /// Gets a hash for this triangle.
        /// </summary>
        public override int GetHashCode()
        {
            return (int)((index2 << 22) ^ (index1 << 11) ^ index0);
        }

        /// <summary>
        /// Checks this triangle and another are equal in value.
        /// </summary>
        /// <param name="tri">The triangle to compare with.</param>
        /// <returns>True if they represent the same value.</returns>
        public bool Equals(Triangle tri)
        {
            return
                index0 == tri.index0 &&
                index1 == tri.index1 &&
                index2 == tri.index2;
        }
        
        public override bool Equals(object obj)
        {
            return (obj is Triangle) && Equals((Triangle)obj);
        }

        public static bool operator ==(Triangle lhs, Triangle rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Triangle lhs, Triangle rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}