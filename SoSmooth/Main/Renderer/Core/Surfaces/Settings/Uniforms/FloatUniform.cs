using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL float uniform.
    /// </summary>
    public class FloatUniform : Uniform<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatUniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        public FloatUniform(string name) : base(name, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatUniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="value">The initial value of the uniform.</param>
        public FloatUniform(string name, float value) : base(name, value) { }
        
        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public override void SetUniform(int location)
        {
            GL.Uniform1(location, Value);
        }
    }
}
