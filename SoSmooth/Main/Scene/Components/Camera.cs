using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Represents a scene camera.
    /// </summary>
    public sealed class Camera : Component
    {
        private Color4 m_clearColor;
        private float m_fieldOfView;
        private float m_nearClip;
        private float m_farClip;

        private int m_resolutionX;
        private int m_resolutionY;
        
        private Matrix4 m_projectionMat;
        private bool m_projectionMatDirty;
        
        private Vector4[] m_frustumPlanes;
        private bool m_planesDirty;

        private UniformBuffer<CameraData> m_dataBuffer;
        private UniformBuffer<LightData> m_lightBuffer;

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
                    m_planesDirty = true;
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
                    m_planesDirty = true;
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
                    m_planesDirty = true;
                }
            }
        }

        /// <summary>
        /// The view matrix of this camera.
        /// </summary>
        public Matrix4 ViewMatrix
        {
            get { return Transform.WorldToLocalMatrix; }
        }

        /// <summary>
        /// The projection matrix of this camera.
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
                        out m_projectionMat);

                    m_projectionMatDirty = false;
                }
                return m_projectionMat;
            }
        }

        /// <summary>
        /// The world space frustum planes of this camera.
        /// </summary>
        public Vector4[] FrustumPlanes
        {
            get
            {
                if (m_planesDirty)
                {
                    ComputeFrustumPlanes();
                }
                return m_frustumPlanes;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Camera(Entity entity) : base(entity)
        {
            m_clearColor = new Color4(0.225f, 0.225f, 0.225f, 1.0f);
            m_fieldOfView = 60.0f;
            m_nearClip = 0.01f;
            m_farClip = 100;

            m_projectionMatDirty = true;
            m_planesDirty = true;

            m_frustumPlanes = new Vector4[6];

            Transform.Moved += OnCameraMoved;
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
                m_planesDirty = true;
            }
            if (m_resolutionY != resY)
            {
                m_resolutionY = resY;
                m_projectionMatDirty = true;
                m_planesDirty = true;
            }
        }

        /// <summary>
        /// Gets the rendering resolution of this camera.
        /// </summary>
        /// <param name="resX">The horizontal rendering resolution in pixels.</param>
        /// <param name="resY">The vertical rendering resolution in pixels.</param>
        public void GetResolution(out int resX, out int resY)
        {
            resX = m_resolutionX;
            resY = m_resolutionY;
        }
        
        /// <summary>
        /// Renders this camera.
        /// </summary>
        public void Render()
        {
            // set up the camera data buffer fo the render
            if (m_dataBuffer == null)
            {
                m_dataBuffer = new UniformBuffer<CameraData>();
            }

            m_dataBuffer.Value = new CameraData(ViewMatrix, ProjectionMatrix);
            m_dataBuffer.BufferData();
            
            // setup the lighting data buffer for the render
            if (m_lightBuffer == null)
            {
                m_lightBuffer = new UniformBuffer<LightData>();
            }

            m_lightBuffer.Value = Entity.Scene.GetLightData();
            m_lightBuffer.BufferData();

            // clear the render target
            GL.ClearColor(m_clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // do culling on all renderable components
            List<Renderable> toRender = new List<Renderable>();
            foreach (Renderable renderable in Entity.Scene.GetRenderables())
            {
                if (!renderable.IsCulled(this))
                {
                    toRender.Add(renderable);
                }
            }
            
            // sort the rendered components to minimize state changes
            toRender.Sort();

            // rendering all active components
            foreach (Renderable renderable in toRender)
            {
                renderable.Render(this);
            }
        }

        /// <summary>
        /// Called when the camera has moved in the scene.
        /// </summary>
        private void OnCameraMoved()
        {
            m_planesDirty = true;
        }

        /// <summary>
        /// Computes the camera frustum planes in world space.
        /// </summary>
        private void ComputeFrustumPlanes()
        {
            // clear the existing plane values
            for (int i = 0; i < m_frustumPlanes.Length; i++)
            {
                m_frustumPlanes[i] = new Vector4();
            }

            Matrix4 viewProj = ViewMatrix * ProjectionMatrix;
            viewProj.Transpose();

            // Left clipping plane
            m_frustumPlanes[0].X = viewProj.M41 + viewProj.M11;
            m_frustumPlanes[0].Y = viewProj.M42 + viewProj.M12;
            m_frustumPlanes[0].Z = viewProj.M43 + viewProj.M13;
            m_frustumPlanes[0].W = viewProj.M44 + viewProj.M14;
            // Right clipping plane
            m_frustumPlanes[1].X = viewProj.M41 - viewProj.M11;
            m_frustumPlanes[1].Y = viewProj.M42 - viewProj.M12;
            m_frustumPlanes[1].Z = viewProj.M43 - viewProj.M13;
            m_frustumPlanes[1].W = viewProj.M44 - viewProj.M14;
            // Top clipping plane
            m_frustumPlanes[2].X = viewProj.M41 - viewProj.M21;
            m_frustumPlanes[2].Y = viewProj.M42 - viewProj.M22;
            m_frustumPlanes[2].Z = viewProj.M43 - viewProj.M23;
            m_frustumPlanes[2].W = viewProj.M44 - viewProj.M24;
            // Bottom clipping plane
            m_frustumPlanes[3].X = viewProj.M41 + viewProj.M21;
            m_frustumPlanes[3].Y = viewProj.M42 + viewProj.M22;
            m_frustumPlanes[3].Z = viewProj.M43 + viewProj.M23;
            m_frustumPlanes[3].W = viewProj.M44 + viewProj.M24;
            // Near clipping plane
            m_frustumPlanes[4].X = viewProj.M41 + viewProj.M31;
            m_frustumPlanes[4].Y = viewProj.M42 + viewProj.M32;
            m_frustumPlanes[4].Z = viewProj.M43 + viewProj.M33;
            m_frustumPlanes[4].W = viewProj.M44 + viewProj.M34;
            // Far clipping plane
            m_frustumPlanes[5].X = viewProj.M41 - viewProj.M31;
            m_frustumPlanes[5].Y = viewProj.M42 - viewProj.M32;
            m_frustumPlanes[5].Z = viewProj.M43 - viewProj.M33;
            m_frustumPlanes[5].W = viewProj.M44 - viewProj.M34;

            // normalize the planes
            for (int i = 0; i < m_frustumPlanes.Length; i++)
            {
                m_frustumPlanes[i] = Utils.NormalizePlane(m_frustumPlanes[i]);
            }

            m_planesDirty = false;
        }
    }
}
