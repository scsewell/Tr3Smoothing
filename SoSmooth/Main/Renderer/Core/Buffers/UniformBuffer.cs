using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Manages a uniform buffer object.
    /// </summary>
    public class UniformBuffer<Data> : Buffer<Data> where Data : struct
    {
        /// <summary>
        /// Initialises a new <see cref="UniformBuffer{Data}"/> instance.
        /// </summary>
        public UniformBuffer() : base(BufferTarget.UniformBuffer, 1)
        {
        }
    }
}
