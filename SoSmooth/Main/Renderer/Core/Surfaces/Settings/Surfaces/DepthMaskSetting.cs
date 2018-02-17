using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a depth mask surface setting.
    /// </summary>
    public class DepthMaskSetting : SurfaceSetting
    {
        private static bool m_initialized = false;
        private static bool m_currentWriteDepth;

        /// <summary>
        /// If true this surface will write to the depth buffer. Default is <see cref="true"/>
        /// </summary>
        public bool WriteDepth = true;
        
        /// <summary>
        /// Sets the values before any upcoming draw calls.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            if (!m_initialized || WriteDepth != m_currentWriteDepth)
            {
                m_currentWriteDepth = WriteDepth;
                m_initialized = true;

                GL.DepthMask(m_currentWriteDepth);
            }
        }
    }
}
