using System;
using System.Collections.Generic;
using System.Text;
using SoSmooth.IO.Vrml.Fields;
using SoSmooth.IO.Vrml.Parser;

namespace SoSmooth.IO.Vrml.Nodes
{
    /// <summary>
    /// Represents a VRML scene node.
    /// </summary>
    public abstract class Node
    {
        public string Name { get; set; }
        public Node Parent { get; set; }
        
        private readonly Dictionary<string, Field> m_fields = new Dictionary<string, Field>();
        
        protected void AddExposedField(string exposedFieldName, Field field)
        {
            AddField(exposedFieldName, field);
        }

        protected void AddField(string fieldName, Field field)
        {
            m_fields[fieldName] = field;
        }

        public Field GetExposedField(string exposedFieldName)
        {
            return (GetField(exposedFieldName));
        }

        public Field GetField(string fieldName)
        {
            if (m_fields.TryGetValue(fieldName, out Field res))
            {
                return res;
            }
            throw new InvalidFieldException($"'{fieldName}' field doesn't exist in node of {GetType().Name} type");
        }
        
        protected abstract Node CreateInstance();

        public Node Clone()
        {
            Node clone = CreateInstance();
            clone.Name = Name;
            CloneFields(m_fields, clone.m_fields);
            return clone;
        }

        private void CloneFields(Dictionary<string, Field> source, Dictionary<string, Field> dest)
        {
            foreach (string key in source.Keys)
            {
                Field field = source[key];
                dest[key] = field.Clone();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            FieldsToString(m_fields, sb);

            return $"{GetType().Name}: {{{Environment.NewLine}{sb}}}";
        }

        private void FieldsToString(Dictionary<string, Field> source, StringBuilder sb)
        {
            foreach (string key in source.Keys)
            {
                if (sb.Length != 0)
                {
                    sb.AppendLine(", ");
                }
                sb.Append(key);
                sb.Append(": ");
                sb.Append(source[key]);
            }
        }
    }
}
