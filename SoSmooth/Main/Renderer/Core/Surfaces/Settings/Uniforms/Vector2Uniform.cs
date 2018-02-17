using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL Vector2 uniform.
    /// </summary>
    public class Vector2Uniform : Uniform<Vector2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        public Vector2Uniform(string name) : base(name, Vector2.Zero) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="vector">The initial value of the uniform.</param>
        public Vector2Uniform(string name, Vector2 vector) : base(name, vector) { }
        
        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public override void SetUniform(int location)
        {
            GL.Uniform2(location, ref Value);
        }
    }
}
