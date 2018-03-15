using System;
using System.Diagnostics;
using Gdk;
using Gtk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// A 3D scene window in the application that manages an OpenGL context.
    /// </summary>
    public class SceneWindow : GLWidget
    {
        private const int OPENGL_VERSION_MAJOR  = 3;
        private const int OPENGL_VERSION_MINOR  = 3;

        private const int DEPTH_BUFFER_SIZE     = 24;
        private const int AA_SAMPLES            = 8;
        
        private Scene m_scene;
        private SceneCamera m_sceneCamera;
        private SceneMeshes m_sceneMeshes;
        private Stopwatch m_stopwatch = new Stopwatch();
        private float m_avgRenterTime = 0;
        private bool m_viewportDirty = true;
        private bool m_cursorOver = false;
        
        /// <summary>
        /// The average render time in seconds.
        /// </summary>
        public float RenderTime { get { return m_avgRenterTime; } }

        /// <summary>
        /// The scene displayed in this window.
        /// </summary>
        public Scene Scene { get { return m_scene; } }

        /// <summary>
        /// The camera in this window.
        /// </summary>
        public SceneCamera Camera { get { return m_sceneCamera; } }

        /// <summary>
        /// The meshes in this window.
        /// </summary>
        public SceneMeshes Meshes { get { return m_sceneMeshes; } }

        /// <summary>
        /// Event triggered before rendering the scene.
        /// </summary>
        public event System.Action Update;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneWindow"/> class.
        /// </summary>
        /// <returns>A new <see cref="SceneWindow"/></returns>
        public static SceneWindow CreateSceneWindow()
        {
            Logger.Info($"Requesting new OpenGL version {OPENGL_VERSION_MAJOR}.{OPENGL_VERSION_MINOR} context");

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
            ) : base(graphicsMode, glVersionMajor, glVersionMinor, contextFlags)
        {
            Name = "SceneWindow";
            CanFocus = true;
            CanDefault = true;
            ReceivesDefault = true;

            Initialized += GLWidgetInitialize;
            SizeAllocated += OnResize;
            
            AddEvents((int)(
                EventMask.EnterNotifyMask |
                EventMask.LeaveNotifyMask |
                EventMask.KeyPressMask |
                EventMask.KeyReleaseMask |
                EventMask.ButtonPressMask |
                EventMask.ButtonReleaseMask |
                EventMask.Button2MotionMask |
                EventMask.ScrollMask
            ));
            
            KeyPressEvent += OnKeyPress;
            ButtonPressEvent += OnButtonPress;
            MotionNotifyEvent += OnMotion;
            ScrollEvent += OnScroll;
        }

        /// <summary>
        /// Called when the context is initialized.
        /// </summary>
        private void GLWidgetInitialize(object sender, EventArgs e)
        {
            // report system OpenGL information
            const int padding = 20;
            string vendor = "Vendor:".PadRight(padding) + GL.GetString(StringName.Vendor);
            string renderer = "Renderer:".PadRight(padding) + GL.GetString(StringName.Renderer);
            string glVersion = "OpenGL Version:".PadRight(padding) + GL.GetString(StringName.Version);
            string slVersion = "GLSL Version:".PadRight(padding) + GL.GetString(StringName.ShadingLanguageVersion);
            Logger.Info($"System OpenGL information: \n{vendor}\n{renderer}\n{glVersion}\n{slVersion}");

            // report the context version
            int major = GL.GetInteger(GetPName.MajorVersion);
            int minor = GL.GetInteger(GetPName.MinorVersion);

            if (major < OPENGL_VERSION_MAJOR || (major == OPENGL_VERSION_MAJOR && minor < OPENGL_VERSION_MINOR))
            {
                Logger.Warning($"Created context is lower than requested version, requested {OPENGL_VERSION_MAJOR}.{OPENGL_VERSION_MINOR} but got {major}.{minor}");
            }

            // disable vsync as we don't want the main thread to be blocked
            GraphicsContext.CurrentContext.SwapInterval = 0;

            // preload all shader programs
            ShaderManager.Instance.LoadShaders();
            
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            
            m_scene = new Scene();
            m_sceneCamera = new SceneCamera(this);
            m_sceneMeshes = new SceneMeshes(this);
        }

        /// <summary>
        /// Called when the widget has been resized. We get the new size and mark that we need
        /// to update the window size next time the scene is rendered.
        /// </summary>
        private void OnResize(object o, SizeAllocatedArgs args)
        {
            m_viewportDirty = true;
        }
        
        /// <summary>
        /// Renders the current scene.
        /// </summary>
        protected override void OnRenderFrame()
        {
            try
            {
                Update?.Invoke();

                // resize the view if it has been changed
                if (m_viewportDirty)
                {
                    GL.Viewport(0, 0, Allocation.Width, Allocation.Height);
                    GL.Scissor(0, 0, Allocation.Width, Allocation.Height);
                    m_sceneCamera.Camera.SetResolution(Allocation.Width, Allocation.Height);
                    m_viewportDirty = false;
                }

                m_stopwatch.Restart();
                m_sceneCamera.RenderScene();
                m_stopwatch.Stop();

                m_avgRenterTime = Utils.Lerp(m_avgRenterTime, (float)m_stopwatch.ElapsedTicks / Stopwatch.Frequency, 0.1f);

                // report the latest error when rendering the scene, if applicable
                ErrorCode error = GL.GetError();
                if (error != ErrorCode.NoError)
                {
                    Logger.Error("OpenGL Error: " + error);
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        protected override bool OnEnterNotifyEvent(EventCrossing evnt)
        {
            m_cursorOver = true;
            return base.OnEnterNotifyEvent(evnt);
        }

        protected override bool OnLeaveNotifyEvent(EventCrossing evnt)
        {
            m_cursorOver = false;
            return base.OnLeaveNotifyEvent(evnt);
        }

        public void OnKeyPress(object o, Gtk.KeyPressEventArgs args)
        {
            // only capture key presses when the cursor is over the window
            if (m_cursorOver)
            {
                m_sceneCamera.OnKeyPress(args);
                m_sceneMeshes.OnKeyPress(args);
            }
        }

        private void OnButtonPress(object o, ButtonPressEventArgs args)
        {
            // only capture key presses when the cursor is over the window
            if (m_cursorOver)
            {
                m_sceneCamera.OnButtonPress(args);
                m_sceneMeshes.OnButtonPress(args);
            }
        }

        private void OnScroll(object o, ScrollEventArgs args)
        {
            m_sceneCamera.OnScroll(args);
        }

        private void OnMotion(object o, MotionNotifyEventArgs args)
        {
            m_sceneCamera.OnMotion(args);
        }
    }
}
