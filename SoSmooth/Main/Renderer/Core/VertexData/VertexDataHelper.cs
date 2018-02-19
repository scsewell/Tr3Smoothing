using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// This class contains helper types and methods to easily create vertex attribute layouts.
    /// </summary>
    public static class VertexDataHelper
    {
        /// <summary>
        /// Defines how all types that may be used as vertex data should be interpreted.
        /// </summary>
        private static readonly Dictionary<Type, AttributeTypeInfo> m_knownTypes = new Dictionary<Type, AttributeTypeInfo>
        {
            { typeof(byte),         ToInfo(VertexAttribPointerType.UnsignedByte,    1, true) },
            { typeof(sbyte),        ToInfo(VertexAttribPointerType.Byte,            1, true) },

            { typeof(short),        ToInfo(VertexAttribPointerType.Short,           1, false) },
            { typeof(ushort),       ToInfo(VertexAttribPointerType.UnsignedShort,   1, false) },

            { typeof(int),          ToInfo(VertexAttribPointerType.Int,             1, false) },
            { typeof(uint),         ToInfo(VertexAttribPointerType.UnsignedInt,     1, false) },
            
            { typeof(Int2101010),   ToInfo(VertexAttribPointerType.Int2101010Rev,           4, true) },
            { typeof(UInt2101010),  ToInfo(VertexAttribPointerType.UnsignedInt2101010Rev,   4, true) },

            { typeof(Color),        ToInfo(VertexAttribPointerType.UnsignedByte,    4, true) },
            { typeof(Color4),       ToInfo(VertexAttribPointerType.Float,           4, false) },

            { typeof(Half),         ToInfo(VertexAttribPointerType.HalfFloat, 1, false) },
            { typeof(Vector2h),     ToInfo(VertexAttribPointerType.HalfFloat, 2, false) },
            { typeof(Vector3h),     ToInfo(VertexAttribPointerType.HalfFloat, 3, false) },
            { typeof(Vector4h),     ToInfo(VertexAttribPointerType.HalfFloat, 4, false) },

            { typeof(float),        ToInfo(VertexAttribPointerType.Float, 1, false) },
            { typeof(Vector2),      ToInfo(VertexAttribPointerType.Float, 2, false) },
            { typeof(Vector3),      ToInfo(VertexAttribPointerType.Float, 3, false) },
            { typeof(Vector4),      ToInfo(VertexAttribPointerType.Float, 4, false) },

            { typeof(double),       ToInfo(VertexAttribPointerType.Double, 1, false) },
            { typeof(Vector2d),     ToInfo(VertexAttribPointerType.Double, 2, false) },
            { typeof(Vector3d),     ToInfo(VertexAttribPointerType.Double, 3, false) },
            { typeof(Vector4d),     ToInfo(VertexAttribPointerType.Double, 4, false) },
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
                    throw new ArgumentException($"Unknown type \"{field.FieldType.FullName}\" in vertex struct of type \"{vertexType.FullName}\"");
                }
                
                array[i] = new VertexAttribute(field.Name, info.Components, info.Type, vertexSize, offset, info.Normalize);

                offset += Marshal.SizeOf(field.FieldType);
            }
            return array;
        }

        /// <summary>
        /// Stores information about a types numeric interpretation.
        /// </summary>
        private struct AttributeTypeInfo
        {
            public VertexAttribPointerType Type { get; private set; }
            public int Components { get; private set; }
            public bool Normalize { get; private set; }

            public AttributeTypeInfo(VertexAttribPointerType type, int components, bool normalize)
            {
                Type = type;
                Components = components;
                Normalize = normalize;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="AttributeTypeInfo"/>.
        /// </summary>
        /// <param name="type">The type of numeric interpretation in the shader.</param>
        /// <param name="components">The number of components in the type.</param>
        /// <param name="normalize">Whether to normalise the attribute's value when passing it to the shader.</param>
        private static AttributeTypeInfo ToInfo(VertexAttribPointerType type, int components, bool normalize)
        {
            return new AttributeTypeInfo(type, components, normalize);
        }
    }
}
