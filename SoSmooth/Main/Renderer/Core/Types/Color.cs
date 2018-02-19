using System;
using System.Runtime.InteropServices;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// A struct representing a 32-bit rgba color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        /// The color's red value
        /// </summary>
        private byte R;

        /// <summary>
        /// The color's green value
        /// </summary>
        private byte G;

        /// <summary>
        /// The color's blue value
        /// </summary>
        private byte B;

        /// <summary>
        /// The color's alpha value (0 = fully transparent, 255 = fully opaque)
        /// </summary>
        private byte A;
        
        /// <summary>
        /// The color, represented as 32-bit unsigned integer, with its color components in the order ARGB.
        /// </summary>
        public uint ARGB
        {
            get
            {
                return ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | B;
            }
        }

        /// <summary>
        /// Returns the hue of the color in the range 0 to 1.
        /// </summary>
        public float Hue
        {
            get
            {
                float r = R / 255f;
                float g = G / 255f;
                float b = B / 255f;

                float h;

                float max = Math.Max(r, Math.Max(g, b));
                if (max == 0)
                {
                    return 0;
                }

                float min = Math.Min(r, Math.Min(g, b));
                float delta = max - min;

                if (r == max)
                {
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    h = ((b - r) / delta) + 2;
                }
                else
                {
                    h = ((r - g) / delta) + 4;
                }

                h /= 6;

                if (h < 0)
                {
                    h += 1;
                }

                return h;
            }
        }

        /// <summary>
        /// Returns the saturation of the color in the range 0 to 1.
        /// </summary>
        public float Saturation
        {
            get
            {
                byte max = Math.Max(R, Math.Max(G, B));
                if (max == 0)
                {
                    return 0;
                }

                byte min = Math.Min(R, Math.Min(G, B));
                return (float)(max - min) / max;
            }
        }

        /// <summary>
        /// Returns the value (lightness) of the color in the range 0 to 1.
        /// </summary>
        public float Value
        {
            get
            {
                return Math.Max(R, Math.Max(G, B)) / 255f;
            }
        }

        /// <summary>
        /// Creates a new grayscale color.
        /// </summary>
        /// <param name="value">The value (brightness) of the color.</param>
        /// <param name="alpha">The opacity of the color.</param>
        /// <returns>A gray color with the given value and transparency.</returns>
        public Color(byte value = 255, byte alpha = 255) : this(value, value, value, alpha)
        { }

        /// <summary>
        /// Constructs a color from a red, green, blue and alpha value.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs a color from a 4 float color.
        /// </summary>
        /// <param name="color">The color to approximate.</param>
        public Color(OpenTK.Graphics.Color4 color) : this(color.ToArgb())
        { }

        /// <summary>
        /// Constructs a color from a 32-bit unsigned integer including the color components in the order ARGB.
        /// </summary>
        /// <param name="argb">The unsigned integer representing the color.</param>
        public Color(uint argb)
        {
            A = (byte)((argb >> 24) & 255);
            R = (byte)((argb >> 16) & 255);
            G = (byte)((argb >> 8) & 255);
            B = (byte)(argb & 255);
        }

        /// <summary>
        /// Constructs a color from a 32-bit integer including the color components in the order ARGB.
        /// </summary>
        /// <param name="argb">The integer representing the color.</param>
        public Color(int argb)
        {
            A = (byte)((argb >> 24) & 255);
            R = (byte)((argb >> 16) & 255);
            G = (byte)((argb >> 8) & 255);
            B = (byte)(argb & 255);
        }

        /// <summary>
        /// Creates a color from hue, saturation and value.
        /// </summary>
        /// <param name="h">Hue of the color [0,1].</param>
        /// <param name="s">Saturation of the color [0,1].</param>
        /// <param name="v">Value of the color [0,1].</param>
        /// <param name="a">Alpha of the color.</param>
        /// <returns>The constructed color.</returns>
        public static Color FromHSVA(float h, float s, float v, byte a = 255)
        {
            h *= 6;
            float chroma = v * s;
            float x = chroma * (1 - Math.Abs((h % 2) - 1));
            float m = v - chroma;

            float r, g, b;
            if (h < 0 || 6 < h)
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else if (h < 1)
            {
                r = chroma;
                g = x;
                b = 0;
            }
            else if (h < 2)
            {
                r = x;
                g = chroma;
                b = 0;
            }
            else if (h < 3)
            {
                r = 0;
                g = chroma;
                b = x;
            }
            else if (h < 4)
            {
                r = 0;
                g = x;
                b = chroma;
            }
            else if (h < 5)
            {
                r = x;
                g = 0;
                b = chroma;
            }
            else
            {
                r = chroma;
                g = 0;
                b = x;
            }
            return new Color((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255), a);
        }

        /// <summary>
        /// Linearly interpolates between two colors and returns the result.
        /// </summary>
        /// <param name="color0">The first color.</param>
        /// <param name="color1">The second color.</param>
        /// <param name="p">Interpolation parameter, is clamped to [0, 1].</param>
        /// <returns>The interpolated color.</returns>
        public static Color Lerp(Color color0, Color color1, float p)
        {
            if (p <= 0)
            {
                return color0;
            }
            if (p >= 1)
            {
                return color1;
            }

            float q = 1 - p;
            return new Color(
                (byte)(color0.R * q + color1.R * p),
                (byte)(color0.G * q + color1.G * p),
                (byte)(color0.B * q + color1.B * p),
                (byte)(color0.A * q + color1.A * p)
                );
        }
        
        /// <summary>
        /// Checks whether this color is the same as another.
        /// </summary>
        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        /// <summary>
        /// Checks whether this color is the same as another.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Color && Equals((Color)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a hexadecimal <see cref="String"/> that represents this color.
        /// </summary>
        /// <returns>
        /// A hexadecimal <see cref="String"/> that represents this color.
        /// </returns>
        public override string ToString()
        {
            return "#" + ARGB.ToString("X8");
        }

        /// <summary>
        /// Casts the color to equivalent <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns><see cref="System.Drawing.Color"/></returns>
        static public implicit operator System.Drawing.Color(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Casts the color to equivalent <see cref="OpenTK.Graphics.Color"/>
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns><see cref="OpenTK.Graphics.Color4"/></returns>
        static public implicit operator OpenTK.Graphics.Color4(Color color)
        {
            return new OpenTK.Graphics.Color4(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Compares two colors for equality.
        /// </summary>
        static public bool operator == (Color color1, Color color2)
        {
            return color1.Equals(color2);
        }

        /// <summary>
        /// Compares two colors for inequality.
        /// </summary>
        public static bool operator != (Color color1, Color color2)
        {
            return !(color1 == color2);
        }

        /// <summary>
        /// Multiplies all components of the color by a given scalar.
        /// Note that scalar values outside the range of 0 to 1 may result in overflow and cause unexpected results.
        /// </summary>
        static public Color operator *(Color color, float scalar)
        {
            return new Color(
                (byte)(color.R * scalar),
                (byte)(color.G * scalar),
                (byte)(color.B * scalar),
                (byte)(color.A * scalar));
        }

        /// <summary>
        /// Multiplies all components of the color by a given scalar.
        /// Note that scalar values outside the range of 0 to 1 may result in overflow and cause unexpected results.
        /// </summary>
        static public Color operator *(float scalar, Color color)
        {
            return color * scalar;
        }
    }
}
