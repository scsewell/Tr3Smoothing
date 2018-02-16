using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL Color uniform.
    /// </summary>
    public class ColorUniform : Uniform<Color4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorUniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        public ColorUniform(string name) : base(name, Color4.White) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorUniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="value">The initial value of the uniform.</param>
        public ColorUniform(string name, Color4 value) : base(name, value) { }

        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public override void SetUniform(int location)
        {
            GL.Uniform4(location, Value);
        }
    }
}
