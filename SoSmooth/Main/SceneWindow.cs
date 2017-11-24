using System;
using Gtk;
using GLib;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth
{
    /// <summary>
    /// Manages the 3D scene view in the application
    /// </summary>
    public class SceneWindow : IDisposable
    {
        private const int OPENGL_VERSION_MAJOR = 3;
        private const int OPENGL_VERSION_MINOR = 0;

        private const int DEPTH_BUFFER_SIZE     = 24;
        private const int AA_SAMPLES            = 4;

        private int m_glWidth;
        private int m_glHeight;
        private bool m_viewportDirty;

        private GLWidget m_glWidget;
        public Widget Widget { get { return m_glWidget; } }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public SceneWindow()
        {
            GraphicsMode mode = new GraphicsMode(DisplayDevice.Default.BitsPerPixel, DEPTH_BUFFER_SIZE, 0, AA_SAMPLES, 0, 0, false);
            m_glWidget = new GLWidget(mode, OPENGL_VERSION_MAJOR, OPENGL_VERSION_MINOR, GraphicsContextFlags.ForwardCompatible);
            m_glWidget.Name = "glWindow";
            m_glWidget.Initialized += GLWidgetInitialize;
            m_glWidget.SizeAllocated += OnResize;
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
            // Resize the view if it has been changed.
            if (m_viewportDirty)
            {
                ResizeGLContext();
            }
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            GL.Begin(PrimitiveType.Triangles);

            GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(0.0f, 1.0f, 4.0f);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 4.0f);

            GL.End();

            GraphicsContext.CurrentContext.SwapBuffers();
        }

        /// <summary>
        /// Handles resizing the framebuffers.
        /// </summary>
        private void ResizeGLContext()
        {
            GL.Viewport(0, 0, m_glWidth, m_glHeight);

            float aspectRatio = ((float)m_glWidth) / m_glHeight;

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspectRatio, 0.1f, 100.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            m_viewportDirty = false;
        }
    }
}
