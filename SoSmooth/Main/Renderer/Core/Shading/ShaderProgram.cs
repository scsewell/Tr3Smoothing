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

        private readonly bool m_isValid;
        private readonly string m_name;

        /// <summary>
        /// Indicates if this program compiled successfuly.
        /// </summary>
        public bool IsValid
        {
            get
            {
                ValidateDispose();
                return m_isValid;
            }
        }

        /// <summary>
        /// The name of this shader program.
        /// </summary>
        public string Name
        {
            get
            {
                ValidateDispose();
                return m_name;
            }
        }

        /// <summary>
        /// Creates a new shader program.
        /// </summary>
        /// <param name="name">The name of the program.</param>
        /// <param name="shaders">The shaders that make up the program.</param>
        public ShaderProgram(string name, params Shader[] shaders) : this(name, shaders as IEnumerable<Shader>)
        { }

        /// <summary>
        /// Creates a new shader program.
        /// </summary>
        /// <param name="name">The name of the program.</param>
        /// <param name="shaders">The different shaders of the program.</param>
        public ShaderProgram(string name, IEnumerable<Shader> shaders)
        {
            m_name = name;

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
            m_isValid = statusCode == 1;

            if (m_isValid)
            {
                // set the uniform block bindings to the corresponding uniform buffers
                int uniformBlockCount;
                GL.GetProgram(this, GetProgramParameterName.ActiveUniformBlocks, out uniformBlockCount);

                for (int i = 0; i < uniformBlockCount; i++)
                {
                    string blockName = GL.GetActiveUniformBlockName(this, i);
                    int bindingPoint = BlockManager.GetBindingPoint(blockName);

                    GL.UniformBlockBinding(this, i, bindingPoint);
                }
            }
            else
            {
                string info;
                GL.GetProgramInfoLog(this, out info);
                Logger.Error($"Could not link shader program: {info}");
            }
        }
        
        /// <summary>
        /// Sets the vertex attributes.
        /// </summary>
        /// <param name="vertexAttributes">The vertex attributes to set.</param>
        public void SetVertexAttributes(VertexAttribute[] vertexAttributes)
        {
            ValidateDispose();

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
            ValidateDispose();

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
            ValidateDispose();

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
        protected override void OnDispose(bool disposing)
        {
            GL.DeleteProgram(this);

            base.OnDispose(disposing);
        }
    }
}
