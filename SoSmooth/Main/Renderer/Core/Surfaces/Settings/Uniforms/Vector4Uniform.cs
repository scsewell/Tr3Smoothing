using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL Vector4 uniform.
    /// </summary>
    public class Vector4Uniform : Uniform<Vector4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        public Vector4Uniform(string name) : base(name, Vector4.Zero) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="vector">The initial value of the uniform.</param>
        public Vector4Uniform(string name, Vector4 vector) : base(name, vector) { }

        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public override void SetUniform(int location)
        {
            GL.Uniform4(location, ref Value);
        }
    }
}
