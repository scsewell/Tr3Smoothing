using System;
using System.Runtime.InteropServices;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// A struct representing a 32-bit argb colour.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Transparent        = new Color(0x00000000);
        public static readonly Color Pink               = new Color(0xFFFFC0CB);
        public static readonly Color LightPink          = new Color(0xFFFFB6C1);
        public static readonly Color HotPink            = new Color(0xFFFF69B4);
        public static readonly Color DeepPink           = new Color(0xFFFF1493);
        public static readonly Color PaleVioletRed      = new Color(0xFFDB7093);
        public static readonly Color MediumVioletRed    = new Color(0xFFC71585);
        public static readonly Color LightSalmon        = new Color(0xFFFFA07A);
        public static readonly Color Salmon             = new Color(0xFFFA8072);
        public static readonly Color DarkSalmon         = new Color(0xFFE9967A);
        public static readonly Color LightCoral         = new Color(0xFFF08080);
        public static readonly Color IndianRed          = new Color(0xFFCD5C5C);
        public static readonly Color Crimson            = new Color(0xFFDC143C);
        public static readonly Color FireBrick          = new Color(0xFFB22222);
        public static readonly Color DarkRed            = new Color(0xFF8B0000);
        public static readonly Color Red                = new Color(0xFFFF0000);
        public static readonly Color OrangeRed          = new Color(0xFFFF4500);
        public static readonly Color Tomato             = new Color(0xFFFF6347);
        public static readonly Color Coral              = new Color(0xFFFF7F50);
        public static readonly Color DarkOrange         = new Color(0xFFFF8C00);
        public static readonly Color Orange             = new Color(0xFFFFA500);
        public static readonly Color Gold               = new Color(0xFFFFD700);
        public static readonly Color Yellow             = new Color(0xFFFFFF00);
        public static readonly Color LightYellow        = new Color(0xFFFFFFE0);
        public static readonly Color LemonChiffon       = new Color(0xFFFFFACD);
        public static readonly Color LightGoldenrodYellow = new Color(0xFFFAFAD2);
        public static readonly Color PapayaWhip         = new Color(0xFFFFEFD5);
        public static readonly Color Moccasin           = new Color(0xFFFFE4B5);
        public static readonly Color PeachPuff          = new Color(0xFFFFDAB9);
        public static readonly Color PaleGoldenrod      = new Color(0xFFEEE8AA);
        public static readonly Color Khaki              = new Color(0xFFF0E68C);
        public static readonly Color DarkKhaki          = new Color(0xFFBDB76B);
        public static readonly Color Cornsilk           = new Color(0xFFFFF8DC);
        public static readonly Color BlanchedAlmond     = new Color(0xFFFFEBCD);
        public static readonly Color Bisque             = new Color(0xFFFFE4C4);
        public static readonly Color NavajoWhite        = new Color(0xFFFFDEAD);
        public static readonly Color Wheat              = new Color(0xFFF5DEB3);
        public static readonly Color BurlyWood          = new Color(0xFFDEB887);
        public static readonly Color Tan                = new Color(0xFFD2B48C);
        public static readonly Color RosyBrown          = new Color(0xFFBC8F8F);
        public static readonly Color SandyBrown         = new Color(0xFFF4A460);
        public static readonly Color Goldenrod          = new Color(0xFFDAA520);
        public static readonly Color DarkGoldenrod      = new Color(0xFFB8860B);
        public static readonly Color Peru               = new Color(0xFFCD853F);
        public static readonly Color Chocolate          = new Color(0xFFD2691E);
        public static readonly Color SaddleBrown        = new Color(0xFF8B4513);
        public static readonly Color Sienna             = new Color(0xFFA0522D);
        public static readonly Color Brown              = new Color(0xFFA52A2A);
        public static readonly Color Maroon             = new Color(0xFF800000);
        public static readonly Color DarkOliveGreen     = new Color(0xFF556B2F);
        public static readonly Color Olive              = new Color(0xFF808000);
        public static readonly Color OliveDrab          = new Color(0xFF6B8E23);
        public static readonly Color YellowGreen        = new Color(0xFF9ACD32);
        public static readonly Color LimeGreen          = new Color(0xFF32CD32);
        public static readonly Color Lime               = new Color(0xFF00FF00);
        public static readonly Color LawnGreen          = new Color(0xFF7CFC00);
        public static readonly Color Chartreuse         = new Color(0xFF7FFF00);
        public static readonly Color GreenYellow        = new Color(0xFFADFF2F);
        public static readonly Color SpringGreen        = new Color(0xFF00FF7F);
        public static readonly Color MediumSpringGreen  = new Color(0xFF00FA9A);
        public static readonly Color LightGreen         = new Color(0xFF90EE90);
        public static readonly Color PaleGreen          = new Color(0xFF98FB98);
        public static readonly Color DarkSeaGreen       = new Color(0xFF8FBC8F);
        public static readonly Color MediumSeaGreen     = new Color(0xFF3CB371);
        public static readonly Color SeaGreen           = new Color(0xFF2E8B57);
        public static readonly Color ForestGreen        = new Color(0xFF228B22);
        public static readonly Color Green              = new Color(0xFF008000);
        public static readonly Color DarkGreen          = new Color(0xFF006400);
        public static readonly Color MediumAquamarine   = new Color(0xFF66CDAA);
        public static readonly Color Aqua               = new Color(0xFF00FFFF);
        public static readonly Color Cyan               = new Color(0xFF00FFFF);
        public static readonly Color LightCyan          = new Color(0xFFE0FFFF);
        public static readonly Color PaleTurquoise      = new Color(0xFFAFEEEE);
        public static readonly Color Aquamarine         = new Color(0xFF7FFFD4);
        public static readonly Color Turquoise          = new Color(0xFF40E0D0);
        public static readonly Color MediumTurquoise    = new Color(0xFF48D1CC);
        public static readonly Color DarkTurquoise      = new Color(0xFF00CED1);
        public static readonly Color LightSeaGreen      = new Color(0xFF20B2AA);
        public static readonly Color CadetBlue          = new Color(0xFF5F9EA0);
        public static readonly Color DarkCyan           = new Color(0xFF008B8B);
        public static readonly Color Teal               = new Color(0xFF008080);
        public static readonly Color LightSteelBlue     = new Color(0xFFB0C4DE);
        public static readonly Color PowderBlue         = new Color(0xFFB0E0E6);
        public static readonly Color LightBlue          = new Color(0xFFADD8E6);
        public static readonly Color SkyBlue            = new Color(0xFF87CEEB);
        public static readonly Color LightSkyBlue       = new Color(0xFF87CEFA);
        public static readonly Color DeepSkyBlue        = new Color(0xFF00BFFF);
        public static readonly Color DodgerBlue         = new Color(0xFF1E90FF);
        public static readonly Color CornflowerBlue     = new Color(0xFF6495ED);
        public static readonly Color SteelBlue          = new Color(0xFF4682B4);
        public static readonly Color RoyalBlue          = new Color(0xFF4169E1);
        public static readonly Color Blue               = new Color(0xFF0000FF);
        public static readonly Color MediumBlue         = new Color(0xFF0000CD);
        public static readonly Color DarkBlue           = new Color(0xFF00008B);
        public static readonly Color Navy               = new Color(0xFF000080);
        public static readonly Color MidnightBlue       = new Color(0xFF191970);
        public static readonly Color Lavender           = new Color(0xFFE6E6FA);
        public static readonly Color Thistle            = new Color(0xFFD8BFD8);
        public static readonly Color Plum               = new Color(0xFFDDA0DD);
        public static readonly Color Violet             = new Color(0xFFEE82EE);
        public static readonly Color Orchid             = new Color(0xFFDA70D6);
        public static readonly Color Fuchsia            = new Color(0xFFFF00FF);
        public static readonly Color Magenta            = new Color(0xFFFF00FF);
        public static readonly Color MediumOrchid       = new Color(0xFFBA55D3);
        public static readonly Color MediumPurple       = new Color(0xFF9370DB);
        public static readonly Color BlueViolet         = new Color(0xFF8A2BE2);
        public static readonly Color DarkViolet         = new Color(0xFF9400D3);
        public static readonly Color DarkOrchid         = new Color(0xFF9932CC);
        public static readonly Color DarkMagenta        = new Color(0xFF8B008B);
        public static readonly Color Purple             = new Color(0xFF800080);
        public static readonly Color Indigo             = new Color(0xFF4B0082);
        public static readonly Color DarkSlateBlue      = new Color(0xFF483D8B);
        public static readonly Color SlateBlue          = new Color(0xFF6A5ACD);
        public static readonly Color MediumSlateBlue    = new Color(0xFF7B68EE);
        public static readonly Color White              = new Color(0xFFFFFFFF);
        public static readonly Color Snow               = new Color(0xFFFFFAFA);
        public static readonly Color Honeydew           = new Color(0xFFF0FFF0);
        public static readonly Color MintCream          = new Color(0xFFF5FFFA);
        public static readonly Color Azure              = new Color(0xFFF0FFFF);
        public static readonly Color AliceBlue          = new Color(0xFFF0F8FF);
        public static readonly Color GhostWhite         = new Color(0xFFF8F8FF);
        public static readonly Color WhiteSmoke         = new Color(0xFFF5F5F5);
        public static readonly Color Seashell           = new Color(0xFFFFF5EE);
        public static readonly Color Beige              = new Color(0xFFF5F5DC);
        public static readonly Color OldLace            = new Color(0xFFFDF5E6);
        public static readonly Color FloralWhite        = new Color(0xFFFFFAF0);
        public static readonly Color Ivory              = new Color(0xFFFFFFF0);
        public static readonly Color AntiqueWhite       = new Color(0xFFFAEBD7);
        public static readonly Color Linen              = new Color(0xFFFAF0E6);
        public static readonly Color LavenderBlush      = new Color(0xFFFFF0F5);
        public static readonly Color MistyRose          = new Color(0xFFFFE4E1);
        public static readonly Color Gainsboro          = new Color(0xFFDCDCDC);
        public static readonly Color LightGray          = new Color(0xFFD3D3D3);
        public static readonly Color Silver             = new Color(0xFFC0C0C0);
        public static readonly Color DarkGray           = new Color(0xFFA9A9A9);
        public static readonly Color Gray               = new Color(0xFF808080);
        public static readonly Color DimGray            = new Color(0xFF696969);
        public static readonly Color LightSlateGray     = new Color(0xFF778899);
        public static readonly Color SlateGray          = new Color(0xFF708090);
        public static readonly Color DarkSlateGray      = new Color(0xFF2F4F4F);
        public static readonly Color Black              = new Color(0xFF000000);
        
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
        /// Creates a new gray color.
        /// </summary>
        /// <param name="value">The value (brightness) of the color.</param>
        /// <param name="alpha">The opacity of the color.</param>
        /// <returns>A gray colour with the given value and transparency.</returns>
        public static Color GrayScale(byte value, byte alpha = 255)
        {
            return new Color(value, value, value, alpha);
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
