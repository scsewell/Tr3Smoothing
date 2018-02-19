using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents a vertex attribute, defining the layout of <see cref="IVertexData"/> implementations.
    /// </summary>
    public sealed class VertexAttribute
    {
        private readonly string m_name;
        private readonly int m_size;
        private readonly VertexAttribPointerType m_type;
        private readonly bool m_normalize;
        private readonly int m_stride;
        private readonly int m_offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="type">The type.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="normalize">Whether to normalise the attribute's value when passing it to the shader.</param>
        public VertexAttribute(
            string name, 
            int size, 
            VertexAttribPointerType type,
            int stride, 
            int offset, 
            bool normalize = false)
        {
            m_name = name;
            m_size = size;
            m_type = type;
            m_stride = stride;
            m_offset = offset;
            m_normalize = normalize;
        }

        /// <summary>
        /// Sets the attribute for a specific program.
        /// </summary>
        /// <param name="program">The program.</param>
        public void SetAttribute(ShaderProgram program)
        {
            int index = program.GetAttributeLocation(m_name);

            if (index != -1)
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, m_size, m_type, m_normalize, m_stride, m_offset);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{{name: {0}, size: {1}, type: {2}, normalize: {3}, stride: {4}, offset: {5}}}",
                m_name, m_size, m_type, m_normalize, m_stride, m_offset
                );
        }
    }
}
