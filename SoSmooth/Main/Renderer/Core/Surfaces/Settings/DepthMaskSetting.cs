using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This immutable class represents a depth mask surface setting.
    /// </summary>
    public class DepthMaskSetting : SurfaceSetting
    {
        private bool m_maskDepth;

        /// <summary>
        /// Default 'Dont Mask' masking setting.
        /// </summary>
        public static readonly DepthMaskSetting DontMask = new DepthMaskSetting(false);

        private DepthMaskSetting(bool maskDepth = false) : base(true)
        {
            m_maskDepth = maskDepth;
        }

        /// <summary>
        /// Sets the depth masking setting for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            GL.DepthMask(m_maskDepth);
        }

        /// <summary>
        /// Sets depth masking to default (enabled) after draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void UnSet(ShaderProgram program)
        {
            GL.DepthMask(true);
        }
    }
}
