namespace SoSmooth.Rendering
{
    public interface IUniformBuffer : IBuffer
    {
        /// <summary>
        /// The binding point index for this uniform block.
        /// </summary>
        int BindingPoint { get; }
    }
}
