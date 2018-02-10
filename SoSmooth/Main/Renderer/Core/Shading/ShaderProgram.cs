using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderer
{
    /// <summary>
    /// This class represents a GLSL shader program.
    /// </summary>
    public class ShaderProgram : IDisposable
    {
        /// <summary>
        /// The GLSL shader program handle.
        /// </summary>
        public readonly int Handle;

        private readonly Dictionary<string, int> m_attributeLocations = new Dictionary<string, int>();
        private readonly Dictionary<string, int> m_uniformLocations = new Dictionary<string, int>();

        public static ShaderProgram FromFiles(string vertexShaderPath, string fragmentShaderPath)
        {
            return new ShaderProgram(VertexShader.FromFile(vertexShaderPath), FragmentShader.FromFile(fragmentShaderPath));
        }

        public static ShaderProgram FromCode(string vertexShaderCode, string fragmentShaderCode)
        {
            return new ShaderProgram(new VertexShader(vertexShaderCode), new FragmentShader(fragmentShaderCode));
        }

        /// <summary>
        /// Creates a new shader program.
        /// </summary>
        /// <param name="shaders">The different shaders of the program.</param>
        public ShaderProgram(params Shader[] shaders) : this(null, (IEnumerable<Shader>)shaders)
        { }

        public ShaderProgram(IEnumerable<Shader> shaders) : this(null, shaders)
        { }

        public ShaderProgram(Action<ShaderProgram> preLinkAction, params Shader[] shaders)
            : this(preLinkAction, (IEnumerable<Shader>)shaders)
        { }

        /// <summary>
        /// Creates a new shader program.
        /// </summary>
        /// <param name="preLinkAction">An action to perform before linking the shader program.</param>
        /// <param name="shaders">The different shaders of the program.</param>
        public ShaderProgram(Action<ShaderProgram> preLinkAction, IEnumerable<Shader> shaders)
        {
            Handle = GL.CreateProgram();
            
            foreach (Shader shader in shaders)
            {
                GL.AttachShader(this, shader);
            }

            if (preLinkAction != null)
            {
                preLinkAction(this);
            }

            GL.LinkProgram(this);
            foreach (Shader shader in shaders)
            {
                GL.DetachShader(this, shader);
            }

            // throw exception if linking failed
            int statusCode;
            GL.GetProgram(this, GetProgramParameterName.LinkStatus, out statusCode);

            if (statusCode != 1)
            {
                string info;
                GL.GetProgramInfoLog(this, out info);
                Logger.Error(string.Format("Could not link shader: {0}", info));
            }
        }

        /// <summary>
        /// Sets the vertex attributes.
        /// </summary>
        /// <param name="vertexAttributes">The vertex attributes to set.</param>
        public void SetVertexAttributes(VertexAttribute[] vertexAttributes)
        {
            for (int i = 0; i < vertexAttributes.Length; i++)
            {
                vertexAttributes[i].SetAttribute(this);
            }
        }

        /// <summary>
        /// Gets an attribute's location.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The attribute's location, or -1 if not found.</returns>
        public int GetAttributeLocation(string name)
        {
            int i;
            if (!m_attributeLocations.TryGetValue(name, out i))
            {
                i = GL.GetAttribLocation(this, name);
                m_attributeLocations.Add(name, i);
            }
            return i;
        }

        /// <summary>
        /// Gets a uniform's location.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <returns>The uniform's location, or -1 if not found.</returns>
        public int GetUniformLocation(string name)
        {
            int i;
            if (!m_uniformLocations.TryGetValue(name, out i))
            {
                i = GL.GetUniformLocation(this, name);
                m_uniformLocations.Add(name, i);
            }
            return i;
        }

        /// <summary>
        /// Casts the shader program object to its GLSL program object handle, for easy use with OpenGL functions.
        /// </summary>
        /// <param name="program">The program.</param>
        /// <returns>GLSL program object handle.</returns>
        static public implicit operator int(ShaderProgram program)
        {
            return program.Handle;
        }
        
        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ShaderProgram()
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
            
            GL.DeleteProgram(this);

            m_disposed = true;
        }
    }
}