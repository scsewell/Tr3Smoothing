using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderering
{
    /// <summary>
    /// This class represents a GLSL shader.
    /// </summary>
    public abstract class Shader : IDisposable
    {
        private readonly ShaderType m_type;

        /// <summary>
        /// The GLSL shader object handle.
        /// </summary>
        public readonly int Handle;

        public Shader(ShaderType type, string code)
        {
            m_type = type;
            Handle = GL.CreateShader(type);

            GL.ShaderSource(this, code);
            GL.CompileShader(this);

            // throw exception if compile failed
            int statusCode;
            GL.GetShader(this, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                string info;
                GL.GetShaderInfoLog(this, out info);
                Logger.Error(string.Format("Could not load shader: {0}", info));
            }
        }

        /// <summary>
        /// Casts the shader object to its GLSL shader object handle, for easy use with OpenGL functions.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <returns>GLSL shader object handle.</returns>
        public static implicit operator int(Shader shader)
        {
            return shader.Handle;
        }
        
        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }
            if (GraphicsContext.CurrentContext == null || GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }

            GL.DeleteShader(this);

            m_disposed = true;
        }
    }
}
