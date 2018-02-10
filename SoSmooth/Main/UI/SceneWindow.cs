using System;
using Gtk;
using GLib;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// Manages the 3D scene view in the application
    /// </summary>
    public class SceneWindow : IDisposable
    {
        private const int OPENGL_VERSION_MAJOR  = 3;
        private const int OPENGL_VERSION_MINOR  = 0;

        private const int DEPTH_BUFFER_SIZE     = 24;
        private const int AA_SAMPLES            = 4;

        private GLWidget m_glWidget;
        public Widget Widget { get { return m_glWidget; } }

        private int m_glWidth;
        private int m_glHeight;
        private bool m_viewportDirty;

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
        /// Constructor.
        /// </summary>
        public SceneWindow()
        {
            GraphicsMode mode = new GraphicsMode(DisplayDevice.Default.BitsPerPixel, DEPTH_BUFFER_SIZE, 0, AA_SAMPLES, 0, 0, false);
            m_glWidget = new GLWidget(mode, OPENGL_VERSION_MAJOR, OPENGL_VERSION_MINOR, GraphicsContextFlags.ForwardCompatible);
            m_glWidget.Name = "SceneView";
            m_glWidget.Initialized += GLWidgetInitialize;
            m_glWidget.SizeAllocated += OnResize;

            m_viewportDirty = true;

            m_scene = new Scene();
                
            m_glWidget.KeyPressEvent += KeyPressEvent;
            m_glWidget.KeyReleaseEvent += KeyReleaseEvent;
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
        /// Frees managed resources.
        /// </summary>
        public void Dispose()
        {
            if (m_glWidget != null)
            {
                m_glWidget.Dispose();
                m_glWidget = null;
            }
        }

        /// <summary>
        /// Called when the context is initialized.
        /// </summary>
        private void GLWidgetInitialize(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.DarkSlateBlue);
            GL.Enable(EnableCap.DepthTest);

            // Add idle event handler to process rendering whenever and as long as time is availble.
            Idle.Add(new IdleHandler(OnIdleProcessMain));
        }

        /// <summary>
        /// Called when the widget has been resized. We get the new size and mark that we need
        /// to update the window size next time the scene is rendered.
        /// </summary>
        private void OnResize(object o, SizeAllocatedArgs args)
        {
            m_glWidth = args.Allocation.Width;
            m_glHeight = args.Allocation.Height;
            m_viewportDirty = true;
        }

        /// <summary>
        /// Render the scene whenever the main applicatoin is not busy.
        /// </summary>
        private bool OnIdleProcessMain()
        {
            if (m_glWidget == null)
            {
                return false;
            }
            else
            {
                RenderFrame();
                return true;
            }
        }
        
        /// <summary>
        /// Renders a frame.
        /// </summary>
        private void RenderFrame()
        {
            Time.FrameStart();

            // Resize the view if it has been changed.
            if (m_viewportDirty)
            {
                GL.Viewport(0, 0, m_glWidth, m_glHeight);
                m_viewportDirty = false;
            }
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (m_scene != null)
            {
                m_scene.Render(m_glWidth, m_glHeight);
            }

            GraphicsContext.CurrentContext.SwapBuffers();
        }
    }
}
