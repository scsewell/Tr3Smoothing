using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a GLSL shader program.
    /// </summary>
    public class ShaderProgram : GraphicsResource
    {
        private readonly Dictionary<string, int> m_attributeLocations = new Dictionary<string, int>();
        private readonly Dictionary<string, int> m_uniformLocations = new Dictionary<string, int>();

        /// <summary>
        /// Creates a new shader program.
        /// </summary>
        /// <param name="shaders">The different shaders of the program.</param>
        public ShaderProgram(params Shader[] shaders) : this((IEnumerable<Shader>)shaders)
        { }

        /// <summary>
        /// Creates a new shader program.
        /// </summary>
        /// <param name="shaders">The different shaders of the program.</param>
        public ShaderProgram(IEnumerable<Shader> shaders)
        {
            m_handle = GL.CreateProgram();

            foreach (Shader shader in shaders)
            {
                GL.AttachShader(this, shader);
            }

            GL.LinkProgram(this);
            foreach (Shader shader in shaders)
            {
                GL.DetachShader(this, shader);
            }

            // check if linking failed
            int statusCode;
            GL.GetProgram(this, GetProgramParameterName.LinkStatus, out statusCode);

            if (statusCode != 1)
            {
                string info;
                GL.GetProgramInfoLog(this, out info);
                Logger.Error($"Could not link shader: {info}");
            }

            // set the uniform block bindings to the corresponding uniform buffers
            int uniformBlockCount;
            GL.GetProgram(this, GetProgramParameterName.ActiveUniformBlocks, out uniformBlockCount);

            for (int i = 0; i < uniformBlockCount; i++)
            {
                string name = GL.GetActiveUniformBlockName(this, i);
                int bindingPoint = BlockManager.GetBindingPoint(name);

                GL.UniformBlockBinding(this, i, bindingPoint);
            }
        }
        
        /// <summary>
        /// Sets the vertex attributes.
        /// </summary>
        /// <param name="vertexAttributes">The vertex attributes to set.</param>
        public void SetVertexAttributes(VertexAttribute[] vertexAttributes)
        {
            foreach (VertexAttribute attribute in vertexAttributes)
            {
                attribute.SetAttribute(this);
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
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose()
        {
            GL.DeleteProgram(this);
        }
    }
}
