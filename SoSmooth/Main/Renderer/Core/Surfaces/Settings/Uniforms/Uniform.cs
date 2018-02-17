namespace SoSmooth.Rendering
{
    /// <summary>
    /// Base class for uniforms.
    /// </summary>
    public abstract class Uniform<T> : SurfaceSetting
    {
        /// <summary>
        /// The name of the uniform.
        /// </summary>
        private readonly string m_name;

        /// <summary>
        /// The value of the uniform.
        /// </summary>
        public T Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Uniform"/> class.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="value">The default value of the uniform.</param>
        public Uniform(string name, T value)
        {
            m_name = name;
            Value = value;
        }
        
        /// <summary>
        /// Sets the uniform for a shader program. Is called before the draw call.
        /// </summary>
        /// <param name="program">The program.</param>
        public override void Set(ShaderProgram program)
        {
            int location = program.GetUniformLocation(m_name);
            if (location >= 0)
            {
                SetUniform(location);
            }
        }

        /// <summary>
        /// Called to set the uniform's value.
        /// </summary>
        /// <param name="location">The location of the uniform in the program.</param>
        public abstract void SetUniform(int location);
    }
}
