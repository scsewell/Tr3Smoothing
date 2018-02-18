using System;
using System.Collections;
using System.Collections.Generic;
using SoSmooth.IO.Vrml.Parser;

namespace SoSmooth.IO.Vrml.Fields
{
    public abstract class MField : Field
    {
    }

    public abstract class MField<T> : MField, IEnumerable<T>
    {
        private readonly List<T> m_values;

        public virtual IEnumerable<T> Values => m_values;
        public virtual int Length => m_values.Count;

        protected MField()
        {
            m_values = new List<T>();
        }

        protected MField(params T[] items)
        {
            m_values = new List<T>(items);
        }

        public virtual void AppendValue(T value)
        {
            m_values.Add(value);
        }

        public virtual void ClearValues()
        {
            m_values.Clear();
        }

        protected static void ParseMField(ParserContext context, Action<ParserContext> itemParser)
        {
            var next = context.PeekNextToken();
            if (next.Type == VrmlTokenType.OpenBracket)
            {
                context.ReadOpenBracket();
                while (true)
                {
                    next = context.PeekNextToken();
                    if (next.Type == VrmlTokenType.CloseBracket)
                    {
                        break;
                    }
                    itemParser(context);
                }
                context.ReadCloseBracket();
            }
            else
            {
                itemParser(context);
            }
        }

        public T GetValue(int index)
        {
            return m_values[index];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_values.GetEnumerator();
        }

        public override string ToString()
        {
            return $"[{string.Join(", " + Environment.NewLine, Values)}]";
        }
    }
}
