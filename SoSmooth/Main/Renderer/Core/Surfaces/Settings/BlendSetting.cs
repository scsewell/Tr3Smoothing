using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This immutable class represents a blend function surface setting.
    /// </summary>
    public class BlendSetting : SurfaceSetting
    {
        private readonly BlendingFactorSrc m_srcBlend;
        private readonly BlendingFactorDest m_dstBlend;
        private readonly BlendEquationMode m_equation;

        /// <summary>
        /// Default 'Alpha' blend function
        /// </summary>
        public static readonly BlendSetting Alpha = new BlendSetting(
            BlendingFactorSrc.SrcAlpha, 
            BlendingFactorDest.OneMinusSrcAlpha, 
            BlendEquationMode.FuncAdd);

        /// <summary>
        /// Default 'Pre-multiplied Alpha' blend function
        /// </summary>
        public static readonly BlendSetting PremultipliedAlpha = new BlendSetting(
            BlendingFactorSrc.One, 
            BlendingFactorDest.OneMinusSrcAlpha, 
            BlendEquationMode.FuncAdd);

        /// <summary>
        /// Default 'Add' blend function
        /// </summary>
        public static readonly BlendSetting Add = new BlendSetting(
            BlendingFactorSrc.SrcAlpha, 
            BlendingFactorDest.One, 
            BlendEquationMode.FuncAdd);

        /// <summary>
        /// Default 'Substract' blend function
        /// </summary>
        public static readonly BlendSetting Substract = new BlendSetting(
            BlendingFactorSrc.SrcAlpha, 
            BlendingFactorDest.One, 
            BlendEquationMode.FuncReverseSubtract);

        /// <summary>
        /// Default 'Multiply' blend function
        /// </summary>
        public static readonly BlendSetting Multiply = new BlendSetting(
            BlendingFactorSrc.Zero, 
            BlendingFactorDest.SrcColor, 
            BlendEquationMode.FuncAdd);

        /// <summary>
        /// Default 'Minimum' blend function
        /// </summary>
        public static readonly BlendSetting Min = new BlendSetting(
            BlendingFactorSrc.One, 
            BlendingFactorDest.One, 
            BlendEquationMode.Min);

        /// <summary>
        /// Default 'Maximum' blend function
        /// </summary>
        public static readonly BlendSetting Max = new BlendSetting(
            BlendingFactorSrc.One, 
            BlendingFactorDest.One, 
            BlendEquationMode.Max);

        /// <summary>
        /// Initializes a new custom instance of the <see cref="BlendSetting"/> class.
        /// </summary>
        /// <param name="src">The <see cref="BlendingFactorSrc"/>.</param>
        /// <param name="dest">The <see cref="BlendingFactorDest"/>.</param>
        /// <param name="equation">The <see cref="BlendEquationMode"/>.</param>
        public BlendSetting(BlendingFactorSrc src, BlendingFactorDest dest, BlendEquationMode equation) : base(true)
        {
            m_srcBlend = src;
            m_dstBlend = dest;
            m_equation = equation;
        }

        /// <summary>
        /// Enables blending and sets the blend function for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(m_srcBlend, m_dstBlend);
            GL.BlendEquation(m_equation);
        }

        /// <summary>
        /// Disables blending after draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void UnSet(ShaderProgram program)
        {
            GL.Disable(EnableCap.Blend);
        }
    }
}
