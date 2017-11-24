using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace SoSmooth.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ColoredVertex
    {
        public const int Size = (3 + 4) * sizeof(float);

        private readonly Vector3 m_position;
        private readonly Color4 m_color;

        public ColoredVertex(Vector3 position, Color4 color)
        {
            m_position = position;
            m_color = color;
        }
    }
}
