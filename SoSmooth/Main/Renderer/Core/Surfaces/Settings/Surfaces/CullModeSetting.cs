using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Sets the culling mode for a surface.
    /// </summary>
    public class CullModeSetting : SurfaceSetting
    {
        private static bool m_initialized = false;
        private static CullMode m_currentCullMode;

        /// <summary>
        /// The culling mode for this surface. Default is <see cref="CullMode.Off"/>
        /// </summary>
        public CullMode CullMode = CullMode.Off;
        
        /// <summary>
        /// Sets the face culling mode. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            if (!m_initialized || CullMode != m_currentCullMode)
            {
                m_currentCullMode = CullMode;
                m_initialized = true;

                switch (m_currentCullMode)
                {
                    case CullMode.Back:
                        GL.Enable(EnableCap.CullFace);
                        GL.CullFace(CullFaceMode.Back);
                        break;
                    case CullMode.Front:
                        GL.Enable(EnableCap.CullFace);
                        GL.CullFace(CullFaceMode.Front);
                        break;
                    case CullMode.Off:
                        GL.Disable(EnableCap.CullFace);
                        break;
                }
            }
        }
    }
}
