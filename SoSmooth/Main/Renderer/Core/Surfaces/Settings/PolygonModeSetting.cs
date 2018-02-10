using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderer
{
    public class PolygonModeSetting : SurfaceSetting
    {
        private readonly PolygonMode m_mode;
        private readonly MaterialFace m_face;

        public PolygonModeSetting(PolygonMode mode) : this(mode, MaterialFace.FrontAndBack)
        { }

        public PolygonModeSetting(PolygonMode mode, MaterialFace face) : base(true)
        {
            m_mode = mode;
            m_face = face;
        }

        public override void Set(ShaderProgram program)
        {
            GL.PolygonMode(m_face, m_mode);
        }

        public override void UnSet(ShaderProgram program)
        {
            GL.PolygonMode(m_face, PolygonMode.Fill);
        }
    }
}

