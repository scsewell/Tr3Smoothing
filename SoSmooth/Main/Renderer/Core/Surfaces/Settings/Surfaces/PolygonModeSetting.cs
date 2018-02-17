using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Sets how polygons will be rendererd.
    /// </summary>
    public class PolygonModeSetting : SurfaceSetting
    {
        private static bool m_initialized = false;
        private static PolygonMode m_currentFrontFaceMode;
        private static PolygonMode m_currentBackFaceMode;

        /// <summary>
        /// The front face drawing mode. Default is <see cref="PolygonMode.Fill"/>
        /// </summary>
        public PolygonMode FrontFaceMode = PolygonMode.Fill;

        /// <summary>
        /// The back face drawing mode. Default is <see cref="PolygonMode.Fill"/>
        /// </summary>
        public PolygonMode BackFaceMode = PolygonMode.Fill;
        
        /// <summary>
        /// Sets the values before any upcoming draw calls.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            if (!m_initialized ||
                FrontFaceMode != m_currentFrontFaceMode ||
                BackFaceMode != m_currentBackFaceMode)
            {
                m_currentFrontFaceMode = FrontFaceMode;
                m_currentBackFaceMode = BackFaceMode;
                m_initialized = true;

                if (m_currentFrontFaceMode != m_currentBackFaceMode)
                {
                    GL.PolygonMode(MaterialFace.Front, m_currentFrontFaceMode);
                    GL.PolygonMode(MaterialFace.Back, m_currentBackFaceMode);
                }
                else
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, m_currentFrontFaceMode);
                }
            }
        }
    }
}

