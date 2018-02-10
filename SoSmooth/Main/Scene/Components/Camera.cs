﻿using System;
using OpenTK;

namespace SoSmooth.Scene
{
    /// <summary>
    /// Represents a scene camera.
    /// </summary>
    public class Camera : Component
    {
        private float m_fieldOfView = 60.0f;

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
        
        private float m_nearClip = 0.01f;

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

        private float m_farClip = 100;

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

        private Matrix4 m_projectionMatrix;
        private bool m_projectionMatDirty = true;

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
                        1.5f,
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
    }
}