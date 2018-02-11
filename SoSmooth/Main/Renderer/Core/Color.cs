using System;
using System.Runtime.InteropServices;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// A struct representing a 32-bit argb colour.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color : IEquatable<Color>
    {
        private byte m_r, m_g, m_b, m_a;

        /// <summary>
        /// The colour's red value
        /// </summary>
        public byte R
        {
            get { return m_r; }
            set { m_r = value; }
        }

        /// <summary>
        /// The colour's green value
        /// </summary>
        public byte G
        {
            get { return m_g; }
            set { m_g = value; }
        }

        /// <summary>
        /// The colour's blue value
        /// </summary>
        public byte B
        {
            get { return m_b; }
            set { m_b = value; }
        }

        /// <summary>
        /// The colour's alpha value (0 = fully transparent, 255 = fully opaque)
        /// </summary>
        public byte A
        {
            get { return m_a; }
            set { m_a = value; }
        }

        /// <summary>
        /// The colour, represented as 32-bit unsigned integer, with its colour components in the order ARGB.
        /// </summary>
        public uint ARGB
        {
            get
            {
                return ((uint)m_a << 24) | ((uint)m_r << 16) | ((uint)m_g << 8) | m_b;
            }
        }

        /// <summary>
        /// Returns the hue of the colour in the range 0 to 1.
        /// </summary>
        public float Hue
        {
            get
            {
                float r = m_r / 255f;
                float g = m_g / 255f;
                float b = m_b / 255f;

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
        /// Returns the saturation of the colour in the range 0 to 1.
        /// </summary>
        public float Saturation
        {
            get
            {
                byte max = Math.Max(m_r, Math.Max(m_g, m_b));
                if (max == 0)
                {
                    return 0;
                }

                byte min = Math.Min(m_r, Math.Min(m_g, m_b));
                return (float)(max - min) / max;
            }
        }

        /// <summary>
        /// Returns the value (lightness) of the colour in the range 0 to 1.
        /// </summary>
        public float Value
        {
            get
            {
                return Math.Max(m_r, Math.Max(m_g, m_b)) / 255f;
            }
        }

        /// <summary>
        /// Creates a new grayscale color.
        /// </summary>
        /// <param name="value">The value (brightness) of the color.</param>
        /// <param name="alpha">The opacity of the color.</param>
        /// <returns>A gray colour with the given value and transparency.</returns>
        public Color(byte value = 255, byte alpha = 255) : this(value, value, value, alpha)
        { }

        /// <summary>
        /// Constructs a colour from a red, green, blue and alpha value.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            m_r = r;
            m_g = g;
            m_b = b;
            m_a = a;
        }

        /// <summary>
        /// Constructs a colour from a 4 float color.
        /// </summary>
        /// <param name="color">The color to approximate.</param>
        public Color(OpenTK.Graphics.Color4 color) : this(color.ToArgb())
        { }

        /// <summary>
        /// Constructs a colour from a 32-bit unsigned integer including the colour components in the order ARGB.
        /// </summary>
        /// <param name="argb">The unsigned integer representing the colour.</param>
        public Color(uint argb)
        {
            m_a = (byte)((argb >> 24) & 255);
            m_r = (byte)((argb >> 16) & 255);
            m_g = (byte)((argb >> 8) & 255);
            m_b = (byte)(argb & 255);
        }

        /// <summary>
        /// Constructs a colour from a 32-bit integer including the colour components in the order ARGB.
        /// </summary>
        /// <param name="argb">The integer representing the colour.</param>
        public Color(int argb)
        {
            m_a = (byte)((argb >> 24) & 255);
            m_r = (byte)((argb >> 16) & 255);
            m_g = (byte)((argb >> 8) & 255);
            m_b = (byte)(argb & 255);
        }

        /// <summary>
        /// Creates a colour from hue, saturation and value.
        /// </summary>
        /// <param name="h">Hue of the colour [0,1].</param>
        /// <param name="s">Saturation of the colour [0,1].</param>
        /// <param name="v">Value of the colour [0,1].</param>
        /// <param name="a">Alpha of the colour.</param>
        /// <returns>The constructed colour.</returns>
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
        /// Linearly interpolates between two colours and returns the result.
        /// </summary>
        /// <param name="color0">The first colour.</param>
        /// <param name="color1">The second colour.</param>
        /// <param name="p">Interpolation parameter, is clamped to [0, 1].</param>
        /// <returns>The interpolated colour.</returns>
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
                (byte)(color0.m_r * q + color1.m_r * p),
                (byte)(color0.m_g * q + color1.m_g * p),
                (byte)(color0.m_b * q + color1.m_b * p),
                (byte)(color0.m_a * q + color1.m_a * p)
                );
        }
        
        /// <summary>
        /// Checks whether this colour is the same as another.
        /// </summary>
        public bool Equals(Color other)
        {
            return m_r == other.m_r && m_g == other.m_g && m_b == other.m_b && m_a == other.m_a;
        }

        /// <summary>
        /// Checks whether this colour is the same as another.
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
                int hashCode = m_r.GetHashCode();
                hashCode = (hashCode * 397) ^ m_g.GetHashCode();
                hashCode = (hashCode * 397) ^ m_b.GetHashCode();
                hashCode = (hashCode * 397) ^ m_a.GetHashCode();
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
            return System.Drawing.Color.FromArgb(color.m_a, color.m_r, color.m_g, color.m_b);
        }

        /// <summary>
        /// Casts the color to equivalent <see cref="OpenTK.Graphics.Color"/>
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns><see cref="OpenTK.Graphics.Color4"/></returns>
        static public implicit operator OpenTK.Graphics.Color4(Color color)
        {
            return new OpenTK.Graphics.Color4(color.m_r, color.m_g, color.m_b, color.m_a);
        }

        /// <summary>
        /// Compares two colours for equality.
        /// </summary>
        static public bool operator == (Color color1, Color color2)
        {
            return color1.Equals(color2);
        }

        /// <summary>
        /// Compares two colours for inequality.
        /// </summary>
        public static bool operator != (Color color1, Color color2)
        {
            return !(color1 == color2);
        }

        /// <summary>
        /// Multiplies all components of the colour by a given scalar.
        /// Note that scalar values outside the range of 0 to 1 may result in overflow and cause unexpected results.
        /// </summary>
        static public Color operator *(Color color, float scalar)
        {
            return new Color(
                (byte)(color.m_r * scalar),
                (byte)(color.m_g * scalar),
                (byte)(color.m_b * scalar),
                (byte)(color.m_a * scalar));
        }

        /// <summary>
        /// Multiplies all components of the colour by a given scalar.
        /// Note that scalar values outside the range of 0 to 1 may result in overflow and cause unexpected results.
        /// </summary>
        static public Color operator *(float scalar, Color color)
        {
            return color * scalar;
        }
    }
}
