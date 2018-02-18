using System;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Interface for all buffer objects.
    /// </summary>
    public interface IBuffer : IDisposable
    {
        /// <summary>
        /// The number of elements in the buffer.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Binds the buffer object.
        /// </summary>
        void Bind();

        /// <summary>
        /// Unbinds the buffer object.
        /// </summary>
        void Unbind();

        /// <summary>
        /// Uploads the buffer to the GPU.
        /// </summary>
        /// <param name="usageHint">The usage hint.</param>
        void BufferData(BufferUsageHint usageHint = BufferUsageHint.DynamicDraw);
    }
}
