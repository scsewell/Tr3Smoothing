using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Renderer
{
    /// <summary>
    /// This class represents an OpenGL texture.
    /// </summary>
    public sealed class Texture : IDisposable
    {
        /// <summary>
        /// Handle of te OpenGL texture.
        /// </summary>
        public int Handle { get; private set; }

        /// <summary>
        /// Height of the texture.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Width of the texture.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        public Texture()
        {
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int tex;
            GL.GenTextures(1, out tex);

            Handle = tex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="width">The texture's width.</param>
        /// <param name="height">The texture's height.</param>
        public Texture(int width, int height)
        {
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int tex;
            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            // create empty texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            Handle = tex;
            Width = width;
            Height = height;

        }
        
        public Texture(Stream stream, bool preMultiplyAlpha = false) : this(new Bitmap(stream), preMultiplyAlpha, true)
        {
        }
        
        private Texture(Bitmap bitmap, bool preMultiplyAlpha = false, bool disposeBitmap = false)
        {
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int tex;
            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            
            Handle = tex;
            Width = bitmap.Width;
            Height = bitmap.Height;

            System.Drawing.Imaging.BitmapData data =
                bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (preMultiplyAlpha)
            {
                int size = data.Width * data.Height * 4;
                byte[] array = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, array, 0, size);
                for (int i = 0; i < size; i += 4)
                {
                    float alpha = array[i + 3] / 255f;
                    array[i] = (byte)(array[i] * alpha);
                    array[i + 1] = (byte)(array[i + 1] * alpha);
                    array[i + 2] = (byte)(array[i + 2] * alpha);
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, array);
            }
            else
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }

            bitmap.UnlockBits(data);

            if (disposeBitmap)
            {
                bitmap.Dispose();
            }
            
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="path">The image file to load.</param>
        /// <param name="preMultiplyAlpha">If true, the colour values of each pixel are multiplied by its alpha value.</param>
        public Texture(string path, bool preMultiplyAlpha = false)
            : this(new Bitmap(path), preMultiplyAlpha, true)
        {
        }

        /// <summary>
        /// Resizes the texture.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="internalFormat">The new <see cref="PixelInternalFormat"/>.</param>
        public void Resize(int width, int height, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba)
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Sets the texture filtering parameters.
        /// </summary>
        /// <param name="minFilter">The <see cref="TextureMinFilter"/>.</param>
        /// <param name="magFilter">The <see cref="TextureMagFilter"/>.</param>
        /// <param name="wrapS">The horizontal <see cref="TextureWrapMode"/>.</param>
        /// <param name="wrapT">The vertical <see cref="TextureWrapMode"/>.</param>
        public void SetParameters(
            TextureMinFilter minFilter, 
            TextureMagFilter magFilter, 
            TextureWrapMode wrapS,
            TextureWrapMode wrapT)
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapT);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Casts the Texture to its OpenGL handle, for easy use with OpenGL functions.
        /// </summary>
        public static implicit operator int(Texture texture)
        {
            return texture.Handle;
        }
        
        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Texture()
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

            GL.DeleteTexture(this);
            Handle = 0;

            m_disposed = true;
        }
    }
}
