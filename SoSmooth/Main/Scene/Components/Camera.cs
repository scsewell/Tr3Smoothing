using System;
using OpenTK;
using OpenTK.Graphics;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents a scene camera.
    /// </summary>
    public class Camera : Component
    {
        private Color4 m_clearColor;
        private float m_fieldOfView;
        private float m_nearClip;
        private float m_farClip;

        private int m_resolutionX;
        private int m_resolutionY;
        
        private Matrix4 m_projectionMatrix;
        private bool m_projectionMatDirty;
        
        /// <summary>
        /// The background color of the camera.
        /// </summary>
        public Color4 ClearColor
        {
            get { return m_clearColor; }
            set { m_clearColor = value; }
        }

        /// <summary>
        /// The vertical field of view of this camera in degrees.
        /// Has the range (0, 180].
        /// </summary>
        public float FieldOfView
        {
            get { return m_fieldOfView; }
            set
            {
                if (m_fieldOfView != value)
                {
                    m_fieldOfView = MathHelper.Clamp(value, 0.001f, 180);
                    m_projectionMatDirty = true;
                }
            }
        }
        
        /// <summary>
        /// The distance of the near clip plane before which nothing is rendered.
        /// Has the range (0, infinity).
        /// </summary>
        public float NearClip
        {
            get { return m_nearClip; }
            set
            {
                if (m_nearClip != value)
                {
                    m_nearClip = Math.Max(value, 0.001f);
                    m_projectionMatDirty = true;
                }
            }
        }

        /// <summary>
        /// The distance of the far clip plane after which nothing is rendered.
        /// Has the range (0, infinity).
        /// </summary>
        public float FarClip
        {
            get { return m_farClip; }
            set
            {
                if (m_farClip != value)
                {
                    m_farClip = Math.Max(value, 0.001f);
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
            m_clearColor = new Color4(0.3f, 0.35f, 0.4f, 1.0f);
            m_fieldOfView = 60.0f;
            m_nearClip = 0.01f;
            m_farClip = 100;

            m_projectionMatDirty = true;
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
