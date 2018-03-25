using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Sets how polygons will be rendererd.
    /// </summary>
    public class PolygonModeSetting : SurfaceSetting
    {
        private static bool m_initialized = false;
        private static PolygonMode m_currentFaceMode;

        /// <summary>
        /// The face drawing mode. Default is <see cref="PolygonMode.Fill"/>
        /// </summary>
        public PolygonMode FaceMode = PolygonMode.Fill;
        
        /// <summary>
        /// Sets the values before any upcoming draw calls.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            if (!m_initialized || FaceMode != m_currentFaceMode)
            {
                m_currentFaceMode = FaceMode;
                m_initialized = true;

                GL.PolygonMode(MaterialFace.FrontAndBack, m_currentFaceMode);
            }
        }
    }
}

