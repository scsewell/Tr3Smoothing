using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class represents an OpenGL texture.
    /// </summary>
    public sealed class Texture : GraphicsResource
    {
        private PixelInternalFormat m_internalFormat;
        private PixelFormat m_pixelFormat;
        private PixelType m_pixelType;
        
        /// <summary>
        /// Height of the texture.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Width of the texture.
        /// </summary>
        public int Width { get; private set; }
        
        /// <summary>
        /// Creates an empty texture.
        /// </summary>
        /// <param name="width">The texture's width.</param>
        /// <param name="height">The texture's height.</param>
        /// <param name="internalFormat">The texture's internal format.</param>
        /// <param name="pixelFormat">The texture's pixel format.</param>
        /// <param name="pixelType">The texture's pixel data type.</param>
        public Texture(int width, int height, PixelInternalFormat internalFormat, PixelFormat pixelFormat, PixelType pixelType)
        {
            m_internalFormat = internalFormat;
            m_pixelFormat = pixelFormat;
            m_pixelType = pixelType;

            Width = width;
            Height = height;

            m_handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, pixelFormat, pixelType, IntPtr.Zero);

            SetParametersBound(TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear, TextureWrapMode.Repeat, TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Creates a texture from a data stream.
        /// </summary>
        /// <param name="stream">A data stream containing the image.</param>
        /// <param name="preMultiplyAlpha">If true, the colour values of each pixel are multiplied by its alpha value.</param>
        public Texture(Stream stream, bool preMultiplyAlpha = false) : this(new Bitmap(stream), preMultiplyAlpha, true)
        { }

        /// <summary>
        /// Creates a texture from an image at a path.
        /// </summary>
        /// <param name="path">The image file to load.</param>
        /// <param name="preMultiplyAlpha">If true, the colour values of each pixel are multiplied by its alpha value.</param>
        public Texture(string path, bool preMultiplyAlpha = false) : this(new Bitmap(path), preMultiplyAlpha, true)
        { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="bitmap">An image to create the texture with.</param>
        /// <param name="preMultiplyAlpha">If true, the colour values of each pixel are multiplied by its alpha value.</param>
        /// <param name="disposeBitmap">if true the bitmap is disposed after the texture is created.</param>
        private Texture(Bitmap bitmap, bool preMultiplyAlpha = false, bool disposeBitmap = false)
        {
            m_internalFormat = PixelInternalFormat.Rgba;
            m_pixelFormat = PixelFormat.Bgra;
            m_pixelType = PixelType.UnsignedByte;

            Width = bitmap.Width;
            Height = bitmap.Height;

            m_handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this);
            
            System.Drawing.Imaging.BitmapData data = 
                bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            if (preMultiplyAlpha)
            {
                int size = data.Width * data.Height * 4;
                byte[] array = new byte[size];
                Marshal.Copy(data.Scan0, array, 0, size);
                for (int i = 0; i < size; i += 4)
                {
                    float alpha = array[i + 3] / 255f;
                    array[i] = (byte)(array[i] * alpha);
                    array[i + 1] = (byte)(array[i + 1] * alpha);
                    array[i + 2] = (byte)(array[i + 2] * alpha);
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, m_internalFormat, data.Width, data.Height, 0,
                    m_pixelFormat, m_pixelType, array);
            }
            else
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, m_internalFormat, data.Width, data.Height, 0,
                    m_pixelFormat, m_pixelType, data.Scan0);
            }

            bitmap.UnlockBits(data);

            if (disposeBitmap)
            {
                bitmap.Dispose();
            }
            
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            SetParametersBound(TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear, TextureWrapMode.Repeat, TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Resizes the texture.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public void Resize(int width, int height)
        {
            ValidateDispose();

            Width = width;
            Height = height;

            GL.BindTexture(TextureTarget.Texture2D, this);
            GL.TexImage2D(TextureTarget.Texture2D, 0, m_internalFormat, width, height, 0, m_pixelFormat, m_pixelType, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
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
            ValidateDispose();

            GL.BindTexture(TextureTarget.Texture2D, this);
            SetParametersBound(minFilter, magFilter, wrapS, wrapT);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Sets the texture filtering parameters.
        /// </summary>
        /// <param name="minFilter">The <see cref="TextureMinFilter"/>.</param>
        /// <param name="magFilter">The <see cref="TextureMagFilter"/>.</param>
        /// <param name="wrapS">The horizontal <see cref="TextureWrapMode"/>.</param>
        /// <param name="wrapT">The vertical <see cref="TextureWrapMode"/>.</param>
        private void SetParametersBound(
            TextureMinFilter minFilter,
            TextureMagFilter magFilter,
            TextureWrapMode wrapS,
            TextureWrapMode wrapT)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapT);
        }

        /// <summary>
        /// Cleanup unmanaged resources.
        /// </summary>
        protected override void OnDispose(bool disposing)
        {
            GL.DeleteTexture(this);

            base.OnDispose(disposing);
        }
    }
}
