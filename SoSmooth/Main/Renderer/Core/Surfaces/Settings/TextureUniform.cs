using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL sampler uniform.
    /// </summary>
    public class TextureUniform : Uniform<Texture>
    {
        /// <summary>
        /// The <see cref="TextureUnit"/> used by this uniform.
        /// </summary>
        public TextureUnit Target;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureUniform"/> class.
        /// </summary>
        /// <param name="name">The name of this uniform.</param>
        /// <param name="texture">The initial <see cref="Texture"/> value of this uniform.</param>
        /// <param name="target">The initial <see cref="TextureUnit"/> used by this uniform.</param>
        public TextureUniform(string name, Texture texture, TextureUnit target = TextureUnit.Texture0) : base(name, texture)
        {
            Target = target;
        }
        
        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public override void SetUniform(int location)
        {
            GL.ActiveTexture(Target);
            GL.BindTexture(TextureTarget.Texture2D, Value);
            GL.Uniform1(location, Target - TextureUnit.Texture0);
        }
    }
}
