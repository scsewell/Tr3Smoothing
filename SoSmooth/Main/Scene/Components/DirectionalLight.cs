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
        private float m_intensity = 1.0f;
        private Color4 m_mainColor = Color4.White;
        private Color4 m_specularColor = Color4.White;
        
        /// <summary>
        /// The intensity of the light.
        /// </summary>
        public float Intensity = 1.0f;
        public float Intentity
        {
            get
            {
                ValidateDispose();
                return m_intensity;
            }
            set
            {
                ValidateDispose();
                m_intensity = value;
            }
        }

        /// <summary>
        /// The mian diffuse color of the light.
        /// </summary>
        public Color4 MainColor
        {
            get
            {
                ValidateDispose();
                return m_mainColor;
            }
            set
            {
                ValidateDispose();
                m_mainColor = value;
            }
        }

        /// <summary>
        /// The specular highlight color of the light.
        /// </summary>
        public Color4 SpecularColor
        {
            get
            {
                ValidateDispose();
                return m_specularColor;
            }
            set
            {
                ValidateDispose();
                m_specularColor = value;
            }
        }

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
            ValidateDispose();

            float value0 = m_intensity * new Color(m_mainColor).Value;
            float value1 = other.m_intensity * new Color(other.m_mainColor).Value;

            return (value0 - value1) > 0 ? -1 : 1;
        }
    }
}
