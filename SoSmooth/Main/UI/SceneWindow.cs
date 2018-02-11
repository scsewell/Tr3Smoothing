using System;
using System.Diagnostics;
using Gtk;
using GLib;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// Manages a 3D scene window in the application.
    /// </summary>
    public class SceneWindow : IDisposable
    {
        private const int OPENGL_VERSION_MAJOR  = 3;
        private const int OPENGL_VERSION_MINOR  = 0;

        private const int DEPTH_BUFFER_SIZE     = 24;
        private const int AA_SAMPLES            = 4;

        private GLWidget m_glWidget;
        public Widget Widget { get { return m_glWidget; } }
        
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
        /// Constructor.
        /// </summary>
        public SceneWindow()
        {
            GraphicsMode mode = new GraphicsMode(DisplayDevice.Default.BitsPerPixel, DEPTH_BUFFER_SIZE, 0, AA_SAMPLES, 0, 0, false);
            m_glWidget = new GLWidget(mode, OPENGL_VERSION_MAJOR, OPENGL_VERSION_MINOR, GraphicsContextFlags.ForwardCompatible);
            m_glWidget.Name = "SceneView";
            m_glWidget.Initialized += GLWidgetInitialize;
            m_glWidget.SizeAllocated += OnResize;
            m_glWidget.RenderFrame += OnRenderFrame;

            m_glWidget.KeyPressEvent += KeyPressEvent;
            m_glWidget.KeyReleaseEvent += KeyReleaseEvent;

            m_stopwatch = new Stopwatch();

            m_glWidget.CanFocus = true;
        }

        [ConnectBefore]
        private void KeyPressEvent(object o, Gtk.KeyPressEventArgs args)
        {
            Logger.Debug(args.Event.Key.ToString());
        }

        [ConnectBefore]
        private void KeyReleaseEvent(object o, Gtk.KeyReleaseEventArgs args)
        {
            Logger.Debug(args.Event.Key.ToString());
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

            /*
             * TODO: MOVE THIS TO CONSTRUCTOR ONCE OPENGL CALLS ARE ONLY DONE WHEN RENDERING
             */
            m_scene = new Scene();

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
        /// Renders a frame.
        /// </summary>
        protected void OnRenderFrame(object sender, EventArgs e)
        {
            // measure the render time
            m_stopwatch.Restart();

            Gdk.Rectangle rect = Widget.Allocation;

            // resize the view if it has been changed
            if (m_viewportDirty)
            {
                GL.Viewport(0, 0, rect.Width, rect.Height);
                GL.Scissor(0, 0, rect.Width, rect.Height);
                m_viewportDirty = false;
            }

            // render the scene
            if (m_scene != null)
            {
                m_scene.Render(rect.Width, rect.Height);
            }
            else
            {
                GL.ClearColor(Color.Black);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }

            m_stopwatch.Stop();
        }
        
        /// <summary>
        /// Frees managed resources.
        /// </summary>
        public void Dispose()
        {
            if (m_glWidget != null)
            {
                m_glWidget.Dispose();
                m_glWidget = null;
            }
            if (m_scene != null)
            {
                m_scene = null;
            }
        }
    }
}
