using System;
using System.Collections.Generic;
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
        private bool m_viewportDirty;
        private Stopwatch m_stopwatch;
        private float m_avgRenterTime;

        /// <summary>
        /// The average render time in seconds.
        /// </summary>
        public float RenderTime
        {
            get { return m_avgRenterTime; }
        }

        /// <summary>
        /// The scene displayed in this window.
        /// </summary>
        public Scene Scene
        {
            get { return m_scene; }
            set
            {
                if (m_scene != value)
                {
                    m_scene = value;
                    m_sceneCamera.Camera.Transform.Root.Entity.Scene = value;
                }
            }
        }

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

            Initialized += GLWidgetInitialize;
            SizeAllocated += OnResize;
            
            AddEvents((int)(
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

            m_viewportDirty = true;
            
            m_scene = new Scene();
            m_sceneCamera = new SceneCamera(Scene);
            m_stopwatch = new Stopwatch();
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
            List<Entity> roots = Scene.FindEntities("Root");
            Entity root = roots.Count == 0 ? new Entity(Scene, "Root") : roots[0];
            //root.Transform.LocalRotation = Quaternion.FromEulerAngles(Time.time, 0, 0);

            List<MeshRenderer> renderers = root.GetComponents<MeshRenderer>();
            foreach (Meshes.Mesh mesh in SmoothingManager.Instance.m_meshes)
            {
                mesh.UseColors = Time.time % 2 < 1;
                //mesh.UseColors = true;

                bool foundMesh = false;
                foreach (MeshRenderer renderer in renderers)
                {
                    if (renderer.Mesh == mesh)
                    {
                        foundMesh = true;
                        break;
                    }
                }
                if (!foundMesh)
                {
                    MeshRenderer r = new MeshRenderer(root);
                    r.Mesh = mesh;
                    r.ShaderProgram = ShaderManager.SHADER_LIT;
                    r.BackFaceMode = PolygonMode.Line;
                }
            }
            
            m_stopwatch.Restart();

            // resize the view if it has been changed
            if (m_viewportDirty)
            {
                GL.Viewport(0, 0, Allocation.Width, Allocation.Height);
                GL.Scissor(0, 0, Allocation.Width, Allocation.Height);
                m_sceneCamera.Camera.SetResolution(Allocation.Width, Allocation.Height);
                m_viewportDirty = false;
            }

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

        private void OnKeyPress(object o, Gtk.KeyPressEventArgs args)
        {
            m_sceneCamera.OnKeyPress(args);
        }

        private void OnButtonPress(object o, ButtonPressEventArgs args)
        {
            m_sceneCamera.OnButtonPress(args);
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
