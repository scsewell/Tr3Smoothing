using System;
using OpenTK.Graphics;
using SoSmooth.Rendering;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Component that represents a directional light.
    /// </summary>
    public class DirectionalLight : Component, IComparable<DirectionalLight>
    {
        /// <summary>
        /// The intensity of the light.
        /// </summary>
        public float Intensity = 1.0f;

        /// <summary>
        /// The mian diffuse color of the light.
        /// </summary>
        public Color4 MainColor = Color4.White;

        /// <summary>
        /// The specular highlight color of the light.
        /// </summary>
        public Color4 SpecularColor = Color4.White;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DirectionalLight(Entity entity) : base(entity)
        {
        }

        /// <summary>
        /// Makes directional lights sort by diffuse intensity.
        /// </summary>
        public int CompareTo(DirectionalLight other)
        {
            float value0 = Intensity * new Color(MainColor).Value;
            float value1 = other.Intensity * new Color(other.MainColor).Value;

            return (value0 - value1) > 0 ? -1 : 1;
        }
    }
}
