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

        public delegate void SceneUpdateHandler();

        /// <summary>
        /// Event triggered prior to scene rendering.
        /// </summary>
        public event SceneUpdateHandler SceneUpdate;
        
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
                EventMask.ButtonPressMask |
                EventMask.ButtonReleaseMask |
                EventMask.Button2MotionMask |
                EventMask.ScrollMask
            ));
            
            CanFocus = true;

            m_stopwatch = new Stopwatch();
            m_scene = new Scene();

            new SceneCamera(this);
        }

        /// <summary>
        /// Called when the context is initialized.
        /// </summary>
        private void GLWidgetInitialize(object sender, EventArgs e)
        {
            // disable vsync as we don't want the main thread to be blocked
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
        
        /// <summary>
        /// Renders the current scene.
        /// </summary>
        protected override void OnRenderFrame()
        {
            // notify a scene update
            if (SceneUpdate != null)
            {
                List<Entity> roots = Scene.FindEntities("Root");
                Entity root = roots.Count == 0 ? new Entity(Scene, "Root") : roots[0];

                List<MeshRenderer> renderers = root.GetComponents<MeshRenderer>();
                foreach (Meshes.Mesh mesh in SmoothingManager.Instance.m_meshes)
                {
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
                        r.ShaderProgram = ShaderManager.SHADER_UNLIT;
                    }
                }

                SceneUpdate();
            }

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

            // report the latest error when rendering the scene, if applicable
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Logger.Error("OpenGL Error: " + error);
            }

            m_stopwatch.Stop();
        }
    }
}
