using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL Matrix4 uniform.
    /// </summary>
    public class Matrix4Uniform : Uniform<Matrix4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        public Matrix4Uniform(string name) : base(name, Matrix4.Identity) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="matrix">The initial value of the uniform.</param>
        public Matrix4Uniform(string name, Matrix4 matrix) : base(name, matrix) { }
        
        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public override void SetUniform(int location)
        {
            GL.UniformMatrix4(location, false, ref Value);
        }
    }
}
