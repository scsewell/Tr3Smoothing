using System.Runtime.InteropServices;

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
    }
}