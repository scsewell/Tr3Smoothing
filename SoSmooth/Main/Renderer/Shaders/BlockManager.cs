using System.Collections.Generic;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Manages the binding points for interface blocks.
    /// </summary>
    public static class BlockManager
    {
        /// <summary>
        /// A mapping from uniform interface block names to a target binding point index where
        /// the interface blocks with a matching name will look for the uniform block data.
        /// The value must be less than GL_MAX_UNIFORM_BUFFER_BINDINGS for the device, which is
        /// guarenteed to be at least 72.
        /// </summary>
        private static readonly Dictionary<string, int> m_blockDataToBindingPoint = new Dictionary<string, int>()
        {
            { typeof(CameraData).Name,  0 },
            { typeof(LightData).Name,   1 },
        };

        /// <summary>
        /// Gets the binding point for a uniform interface block.
        /// </summary>
        /// <param name="name">The name of the interface block.</param>
        /// <returns>The binding point index.</returns>
        public static int GetBindingPoint(string name)
        {
            int bindingIndex;
            if (!m_blockDataToBindingPoint.TryGetValue(name, out bindingIndex))
            {
                Logger.Error($"No binding point for interface blocks with the name \"{name}\" was found!");
                bindingIndex = -1;
            }
            return bindingIndex;
        }
    }
}
