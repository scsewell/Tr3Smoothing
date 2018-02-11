using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class contains helper types and methods to easily create vertex attribute layouts.
    /// </summary>
    public static class VertexData
    {
        private static readonly Dictionary<Type, AttributeTypeInfo> m_knownTypes = new Dictionary<Type, AttributeTypeInfo>
        {
                { typeof(byte),     ToInfo(VertexAttribPointerType.UnsignedByte,    true) },
                { typeof(sbyte),    ToInfo(VertexAttribPointerType.Byte,            true) },

                { typeof(short),    ToInfo(VertexAttribPointerType.Short,           false) },
                { typeof(ushort),   ToInfo(VertexAttribPointerType.UnsignedShort,   false) },

                { typeof(int),      ToInfo(VertexAttribPointerType.Int,             false) },
                { typeof(uint),     ToInfo(VertexAttribPointerType.UnsignedInt,     false) },

                { typeof(Color),    ToInfo(VertexAttribPointerType.UnsignedByte, true) },

                { typeof(Half),     ToInfo(VertexAttribPointerType.HalfFloat, false) },
                { typeof(Vector2h), ToInfo(VertexAttribPointerType.HalfFloat, false) },
                { typeof(Vector3h), ToInfo(VertexAttribPointerType.HalfFloat, false) },
                { typeof(Vector4h), ToInfo(VertexAttribPointerType.HalfFloat, false) },

                { typeof(float),    ToInfo(VertexAttribPointerType.Float, false) },
                { typeof(Vector2),  ToInfo(VertexAttribPointerType.Float, false) },
                { typeof(Vector3),  ToInfo(VertexAttribPointerType.Float, false) },
                { typeof(Vector4),  ToInfo(VertexAttribPointerType.Float, false) },

                { typeof(double),   ToInfo(VertexAttribPointerType.Double, false) },
                { typeof(Vector2d), ToInfo(VertexAttribPointerType.Double, false) },
                { typeof(Vector3d), ToInfo(VertexAttribPointerType.Double, false) },
                { typeof(Vector4d), ToInfo(VertexAttribPointerType.Double, false) },
            };

        private static readonly Dictionary<VertexAttribPointerType, int> m_attribByteSizes = new Dictionary<VertexAttribPointerType, int>
            {
                { VertexAttribPointerType.Byte,                     1 },
                { VertexAttribPointerType.UnsignedByte,             1 },
                { VertexAttribPointerType.Fixed,                    1 },
                { VertexAttribPointerType.Short,                    2 },
                { VertexAttribPointerType.UnsignedShort,            2 },
                { VertexAttribPointerType.HalfFloat,                2 },
                { VertexAttribPointerType.Int,                      4 },
                { VertexAttribPointerType.UnsignedInt,              4 },
                { VertexAttribPointerType.Float,                    4 },
                { VertexAttribPointerType.Int2101010Rev,            4 },
                { VertexAttribPointerType.UnsignedInt2101010Rev,    4 },
                { VertexAttribPointerType.Double,                   8 },
            };
        
        /// <summary>
        /// Creates a <see cref="VertexAttribute"/> array from a list of attribute templates.
        /// Offset and stride are calculated automatically, assuming zero padding.
        /// </summary>
        /// <param name="attributes">The attribute templates.</param>
        public static VertexAttribute[] MakeAttributeArray<TVertex>() where TVertex : struct, IVertexData
        {
            Type vertexType = typeof(TVertex);
            int vertexSize = Marshal.SizeOf(vertexType);

            // get all fields in the vertex struct
            FieldInfo[] fields = vertexType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // create array of vertex attributes for each field
            VertexAttribute[] array = new VertexAttribute[fields.Length];

            int offset = 0;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo field = fields[i];

                AttributeTypeInfo info;
                if (!m_knownTypes.TryGetValue(field.FieldType, out info))
                {
                    throw new ArgumentException(string.Format(
                        "Unknown type [{0}] in vertex struct of type [{1}]",
                        field.FieldType.FullName, vertexType.FullName));
                }

                int fieldSize = Marshal.SizeOf(field.FieldType);

                // Gets the number of components in vector types and such
                int count = fieldSize / m_attribByteSizes[info.Type];
                array[i] = new VertexAttribute(field.Name, count, info.Type, vertexSize, offset, info.Normalize);

                offset += fieldSize;
            }
            return array;
        }

        /// <summary>
        /// Stores information about a types numeric interpretation.
        /// </summary>
        private struct AttributeTypeInfo
        {
            public VertexAttribPointerType Type { get; private set; }
            public bool Normalize { get; private set; }

            public AttributeTypeInfo(VertexAttribPointerType type, bool normalize)
            {
                Type = type;
                Normalize = normalize;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="AttributeTypeInfo"/>.
        /// </summary>
        /// <param name="type">The type of numeric interpretation in the shader.</param>
        /// <param name="normalize">Whether to normalise the attribute's value when passing it to the shader.</param>
        private static AttributeTypeInfo ToInfo(VertexAttribPointerType type, bool normalize)
        {
            return new AttributeTypeInfo(type, normalize);
        }
    }
}
