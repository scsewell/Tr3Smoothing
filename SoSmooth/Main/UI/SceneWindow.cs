using System;
using System.Diagnostics;
using Gdk;
using Gtk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// A 3D scene window in the application.
    /// </summary>
    public class SceneWindow : GLWidget
    {
        private const int OPENGL_VERSION_MAJOR  = 3;
        private const int OPENGL_VERSION_MINOR  = 0;

        private const int DEPTH_BUFFER_SIZE     = 24;
        private const int AA_SAMPLES            = 4;
        
        private bool m_viewportDirty;

        private Stopwatch m_stopwatch;
        private Scene m_scene;

        /// <summary>
        /// The scene displayed in this window.
        /// </summary>
        public Scene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }
        
        /// <summary>
        /// How long it took to render the last frame in milliseconds.
        /// </summary>
        public float RenderTime
        {
            get { return 1000 * ((float)m_stopwatch.ElapsedTicks / Stopwatch.Frequency); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneWindow"/> class.
        /// </summary>
        /// <returns>A new <see cref="SceneWindow"/></returns>
        public static SceneWindow CreateSceneWindow()
        {
            GraphicsMode mode = new GraphicsMode(DisplayDevice.Default.BitsPerPixel, DEPTH_BUFFER_SIZE, 0, AA_SAMPLES, 0, 0, false);
            return new SceneWindow(mode, OPENGL_VERSION_MAJOR, OPENGL_VERSION_MINOR, GraphicsContextFlags.ForwardCompatible);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneWindow"/> class.
        /// </summary>
        /// <param name="graphicsMode">The graphics mode.</param>
        /// <param name="glVersionMajor">The OpenGL major version.</param>
        /// <param name="glVersionMinor">The OpenGL minor version.</param>
        /// <param name="contextFlags">The graphics context flags.</param>
        private SceneWindow(
            GraphicsMode graphicsMode,
            int glVersionMajor,
            int glVersionMinor,
            GraphicsContextFlags contextFlags
            )  : base(graphicsMode, glVersionMajor, glVersionMinor, contextFlags)
        {
            Name = "SceneView";
            Initialized += GLWidgetInitialize;
            SizeAllocated += OnResize;
            
            AddEvents((int)(
                EventMask.KeyPressMask |
                EventMask.KeyReleaseMask |
                EventMask.ScrollMask |
                EventMask.Button2MotionMask |
                EventMask.PointerMotionMask |
                EventMask.PointerMotionHintMask
            ));
            
            ButtonPressEvent += M_glWidget_ButtonPressEvent;
            MotionNotifyEvent += M_glWidget_MotionNotifyEvent;
            ScrollEvent += M_glWidget_ScrollEvent;

            CanFocus = true;

            m_stopwatch = new Stopwatch();

            m_scene = new Scene();

            m_camPivot = new Entity(m_scene, "CameraPivot").Transform;

            m_camPitch = new Entity(m_scene, "CameraPitch").Transform;
            m_camPitch.SetParent(m_camPivot);
            
            Transform cam = new Entity(m_scene, "Camera").Transform;
            cam.SetParent(m_camPitch);
            cam.LocalRotation = Quaternion.FromEulerAngles(0, 0, MathHelper.PiOver2);

            m_camera = new Camera(cam.Entity);
            m_scene.ActiveCamera = m_camera;
        }

        /// <summary>
        /// Called when the context is initialized.
        /// </summary>
        private void GLWidgetInitialize(object sender, EventArgs e)
        {
            // disable vsync as we don't want the thread to be blocked needlessly
            GraphicsContext.CurrentContext.SwapInterval = 0;

            // preload all shader programs
            ShaderManager.Instance.LoadShaders();
            
            m_viewportDirty = true;
        }

        /// <summary>
        /// Called when the widget has been resized. We get the new size and mark that we need
        /// to update the window size next time the scene is rendered.
        /// </summary>
        private void OnResize(object o, SizeAllocatedArgs args)
        {
            m_viewportDirty = true;
        }

        private const float ZOOM_MIN = 0.001f;
        private const float ZOOM_MAX = 100.0f;
        private const float ZOOM_RATIO = 1.0f / 8.0f;

        private const float ROTATE_STEP_SIZE = MathHelper.TwoPi / 28;

        private float m_orbitDistance = 10;
        private float m_pitch = 0;
        private float m_yaw = 0;

        private Transform m_camPivot;
        private Transform m_camPitch;
        private Camera m_camera;
        

        private void UpdateScene()
        {
            m_camPivot.LocalRotation = Quaternion.FromEulerAngles(m_yaw, 0, 0);
            m_camPitch.LocalRotation = Quaternion.FromEulerAngles(0, 0, m_pitch);
            m_camera.Transform.LocalPosition = -m_orbitDistance * Vector3.UnitY;

            m_camera.FarClip = Math.Max(m_orbitDistance * 2, 100);
        }

        /// <summary>
        /// Renders the current scene.
        /// </summary>
        protected override void OnRenderFrame()
        {
            UpdateScene();

            m_stopwatch.Restart();

            // resize the view if it has been changed
            if (m_viewportDirty)
            {
                GL.Viewport(0, 0, Allocation.Width, Allocation.Height);
                GL.Scissor(0, 0, Allocation.Width, Allocation.Height);
                m_viewportDirty = false;
            }

            // render the scene
            if (m_scene != null)
            {
                m_scene.Render(Allocation.Width, Allocation.Height);
            }
            else
            {
                GL.ClearColor(Color4.Black);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }

            m_stopwatch.Stop();
        }

        private void M_glWidget_MotionNotifyEvent(object o, MotionNotifyEventArgs args)
        {
            //Logger.Debug(args.Event.Device.Name.ToString());
        }

        private void M_glWidget_ScrollEvent(object o, ScrollEventArgs args)
        {
            float zoomDelta = ZOOM_RATIO * m_orbitDistance;

            switch (args.Event.Direction)
            {
                case ScrollDirection.Up:
                    m_orbitDistance = Math.Max(m_orbitDistance - zoomDelta, ZOOM_MIN);
                    Logger.Debug("m_orbitDistance: " + m_orbitDistance);
                    break;
                case ScrollDirection.Down:
                    m_orbitDistance = Math.Min(m_orbitDistance + zoomDelta, ZOOM_MAX);
                    Logger.Debug("m_orbitDistance: " + m_orbitDistance);
                    break;
            }
        }

        private void M_glWidget_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            //Logger.Debug(args.Event.Device.Name);
        }

        protected override bool OnKeyPressEvent(EventKey evnt)
        {
            switch (evnt.Key)
            {
                case Gdk.Key.KP_1: // look at front
                    m_yaw = 0;
                    m_pitch = 0;
                    break;
                case Gdk.Key.KP_3: // look at side
                    m_yaw = MathHelper.PiOver2;
                    m_pitch = 0;
                    break;
                case Gdk.Key.KP_7: // look at top
                    m_yaw = 0;
                    m_pitch = -MathHelper.PiOver2;
                    break;
                case Gdk.Key.KP_4: m_yaw -= ROTATE_STEP_SIZE; Logger.Debug("m_yaw: " + m_yaw); break;
                case Gdk.Key.KP_6: m_yaw += ROTATE_STEP_SIZE; Logger.Debug("m_yaw: " + m_yaw); break;
                case Gdk.Key.KP_8: m_pitch = Math.Max(m_pitch - ROTATE_STEP_SIZE, -MathHelper.PiOver2); Logger.Debug("m_pitch: " + m_pitch); break;
                case Gdk.Key.KP_2: m_pitch = Math.Min(m_pitch + ROTATE_STEP_SIZE, MathHelper.PiOver2); Logger.Debug("m_pitch: " + m_pitch); break;
            }
            Logger.Debug(evnt.Key.ToString());
            return true;
        }
    }
}
