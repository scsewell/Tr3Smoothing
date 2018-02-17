using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This represents a blend function surface setting.
    /// </summary>
    public class BlendSetting : SurfaceSetting
    {
        private static bool m_initialized = false;
        private static BlendMode m_currentBlendMode;
        
        /// <summary>
        /// The blend mode used to combine the fragment shader's output with the render target.
        /// Default is <see cref="BlendMode.None"/>.
        /// </summary>
        public BlendMode BlendMode = BlendMode.None;
        
        /// <summary>
        /// Enables blending and sets the blend function for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            if (!m_initialized || BlendMode != m_currentBlendMode)
            {
                m_currentBlendMode = BlendMode;
                m_initialized = true;

                if (m_currentBlendMode == BlendMode.None)
                {
                    GL.Disable(EnableCap.Blend);
                }
                else
                {
                    BlendingFactorSrc src;
                    BlendingFactorDest dst;
                    BlendEquationMode eqn;

                    switch (m_currentBlendMode)
                    {
                        case BlendMode.Alpha:
                            src = BlendingFactorSrc.SrcAlpha;
                            dst = BlendingFactorDest.OneMinusSrcAlpha;
                            eqn = BlendEquationMode.FuncAdd;
                            break;
                        case BlendMode.PremultipliedAlpha:
                            src = BlendingFactorSrc.One;
                            dst = BlendingFactorDest.OneMinusSrcAlpha;
                            eqn = BlendEquationMode.FuncAdd;
                            break;
                        case BlendMode.Add:
                            src = BlendingFactorSrc.SrcAlpha;
                            dst = BlendingFactorDest.One;
                            eqn = BlendEquationMode.FuncAdd;
                            break;
                        case BlendMode.Subtract:
                            src = BlendingFactorSrc.SrcAlpha;
                            dst = BlendingFactorDest.One;
                            eqn = BlendEquationMode.FuncReverseSubtract;
                            break;
                        case BlendMode.Multiply:
                            src = BlendingFactorSrc.Zero;
                            dst = BlendingFactorDest.SrcColor;
                            eqn = BlendEquationMode.FuncAdd;
                            break;
                        case BlendMode.Min:
                            src = BlendingFactorSrc.One;
                            dst = BlendingFactorDest.One;
                            eqn = BlendEquationMode.Min;
                            break;
                        case BlendMode.Max:
                            src = BlendingFactorSrc.One;
                            dst = BlendingFactorDest.One;
                            eqn = BlendEquationMode.Max;
                            break;
                        default:
                            throw new System.Exception($"BlendMode \"{m_currentBlendMode}\" needs an equation definition!");
                    }

                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(src, dst);
                    GL.BlendEquation(eqn);
                }
            }
        }
    }
}
