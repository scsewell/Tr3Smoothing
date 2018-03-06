using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL shader.
    /// </summary>
    public abstract class Shader : GraphicsResource
    {
        /// <summary>
        /// Indicates if this shader compiled successfuly.
        /// </summary>
        public readonly bool IsValid;

        private readonly ShaderType m_type;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of shader.</param>
        /// <param name="code">The shader source code.</param>
        public Shader(ShaderType type, string code)
        {
            m_type = type;

            m_handle = GL.CreateShader(type);

            GL.ShaderSource(this, code);
            GL.CompileShader(this);

            // check compile success
            int statusCode;
            GL.GetShader(this, ShaderParameter.CompileStatus, out statusCode);
            IsValid = statusCode == 1;

            if (!IsValid)
            {
                string info;
                GL.GetShaderInfoLog(this, out info);
                Logger.Error(string.Format("Could not load shader: {0}", info));
            }
        }

        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose(bool disposing)
        {
            GL.DeleteShader(this);

            base.OnDispose(disposing);
        }
    }
}
