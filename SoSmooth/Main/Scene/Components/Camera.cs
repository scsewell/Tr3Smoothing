using System;
using OpenTK;
using SoSmooth.Rendering;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents a scene camera.
    /// </summary>
    public class Camera : Component
    {
        private int m_resolutionX;
        private int m_resolutionY;

        private Color m_clearColor = Color.DarkSlateBlue;
        private float m_fieldOfView = 60.0f;
        private float m_nearClip = 0.01f;
        private float m_farClip = 100;

        private Matrix4 m_projectionMatrix;
        private bool m_projectionMatDirty = true;
        
        /// <summary>
        /// The background color of the camera.
        /// </summary>
        public Color ClearColor
        {
            get { return m_clearColor; }
            set { m_clearColor = value; }
        }

        /// <summary>
        /// The vertical field of view of this camera in degrees.
        /// Has the range [0, 180].
        /// </summary>
        public float FieldOfView
        {
            get { return m_fieldOfView; }
            set
            {
                if (m_fieldOfView != value)
                {
                    m_fieldOfView = Math.Min(Math.Max(value, 0), 180);
                    m_projectionMatDirty = true;
                }
            }
        }
        
        /// <summary>
        /// The distance of the near clip plane before which nothing is rendered.
        /// Has the range [0, infinity).
        /// </summary>
        public float NearClip
        {
            get { return m_nearClip; }
            set
            {
                if (m_nearClip != value)
                {
                    m_nearClip = Math.Max(value, 0);
                    m_projectionMatDirty = true;
                }
            }
        }

        /// <summary>
        /// The distance of the far clip plane after which nothing is rendered.
        /// Has the range [NearClip, infinity).
        /// </summary>
        public float FarClip
        {
            get { return m_farClip; }
            set
            {
                if (m_farClip != value)
                {
                    m_farClip = Math.Max(value, NearClip);
                    m_projectionMatDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets the projection matrix of this camera.
        /// </summary>
        public Matrix4 ProjectionMatrix
        {
            get
            {
                if (m_projectionMatDirty)
                {
                    Matrix4.CreatePerspectiveFieldOfView(
                        MathHelper.DegreesToRadians(m_fieldOfView),
                        ((float)m_resolutionX) / m_resolutionY,
                        m_nearClip, 
                        m_farClip,
                        out m_projectionMatrix);

                    m_projectionMatDirty = false;
                }
                return m_projectionMatrix;
            }
        }

        /// <summary>
        /// Gets the view matrix of this camera.
        /// </summary>
        public Matrix4 ViewMatrix
        {
            get { return Transform.WorldToLocalMatrix; }
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public Camera(Entity entity) : base(entity)
        {
        }

        /// <summary>
        /// Sets the rendering resolution of this camera.
        /// </summary>
        /// <param name="resX">The horizontal rendering resolution in pixels.</param>
        /// <param name="resY">The vertical rendering resolution in pixels.</param>
        public void SetResolution(int resX, int resY)
        {
            if (m_resolutionX != resX)
            {
                m_resolutionX = resX;
                m_projectionMatDirty = true;
            }
            if (m_resolutionY != resY)
            {
                m_resolutionY = resY;
                m_projectionMatDirty = true;
            }
        }
    }
}
