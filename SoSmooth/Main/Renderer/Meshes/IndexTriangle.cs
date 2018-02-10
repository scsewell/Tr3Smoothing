using System.Runtime.InteropServices;

namespace SoSmooth.Renderering.Meshes
{
    /// <summary>
    /// A triplet of vertex indices, representing a triangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct IndexTriangle
    {
        public readonly ushort index0;
        public readonly ushort index1;
        public readonly ushort index2;
        
        /// <summary>
        /// Creates a new index triangle.
        /// </summary>
        public IndexTriangle(ushort i0, ushort i1, ushort i2)
        {
            index0 = i0;
            index1 = i1;
            index2 = i2;
        }
    }
}