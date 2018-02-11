using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL Color uniform.
    /// </summary>
    public class ColorUniform : SurfaceSetting
    {
        /// <summary>
        /// The name of the uniform
        /// </summary>
        private string m_name;

        /// <summary>
        /// The <see cref="Color"/> value of the uniform.
        /// </summary>
        public Color4 Color;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorUniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        public ColorUniform(string name) : this(name, Color4.White) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorUniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="color">The initial <see cref="Color"/> value of the uniform.</param>
        public ColorUniform(string name, Color4 color)
        {
            m_name = name;
            Color = color;
        }
        
        /// <summary>
        /// Sets the <see cref="Color"/> uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            GL.Uniform4(program.GetUniformLocation(m_name), Color);
        }
    }
}
