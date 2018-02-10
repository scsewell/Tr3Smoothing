
using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderering
{
    internal class VertexArray<TVertexData> : IVertexAttributeProvider<TVertexData>, IDisposable where TVertexData : struct, IVertexData
    {
        private readonly VertexBuffer<TVertexData> m_vertexBuffer;
        private int m_handle;
        private bool m_vertexArrayGenerated = false;

        public VertexArray(VertexBuffer<TVertexData> vertexBuffer)
        {
            m_vertexBuffer = vertexBuffer;
        }

        public void SetVertexData()
        {
            GL.BindVertexArray(m_handle);
        }

        public void UnSetVertexData()
        {
            GL.BindVertexArray(0);
        }

        public void SetShaderProgram(ShaderProgram program)
        {
            if (m_vertexArrayGenerated)
            {
                GL.DeleteVertexArrays(1, ref m_handle);
            }

            GL.GenVertexArrays(1, out m_handle);
            m_vertexArrayGenerated = true;

            GL.BindVertexArray(m_handle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexBuffer);

            program.SetVertexAttributes(new TVertexData().VertexAttributes());

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        
        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VertexArray()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }
            if (GraphicsContext.CurrentContext == null || GraphicsContext.CurrentContext.IsDisposed)
            {
                return;
            }

            if (m_vertexArrayGenerated)
            {
                GL.DeleteVertexArray(m_handle);
            }

            m_disposed = true;
        }
    }
}
