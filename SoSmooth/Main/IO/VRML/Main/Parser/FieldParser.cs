using System;
using SoSmooth.IO.Vrml.Fields;
using SoSmooth.IO.Vrml.Tokenizer;

namespace SoSmooth.IO.Vrml.Parser
{
    public class FieldParser : IFieldVisitor
    {
        private readonly ParserContext m_context;
        private readonly Action<ParserContext> m_nodeStatementParser;

        public FieldParser(ParserContext context, Action<ParserContext> nodeStatementParser)
        {
            m_context = context;
            m_nodeStatementParser = nodeStatementParser;
        }
        
        public void Visit(SFBool field)
        {
            string value = m_context.ReadNextToken().Text;
            switch (value)
            {
                case "TRUE":
                    field.Value = true;
                    break;
                case "FALSE":
                    field.Value = false;
                    break;
            }
        }

        public void Visit(SFInt32 field)
        {
            field.Value = m_context.ReadInt32();
        }

        public void Visit(SFFloat field)
        {
            field.Value = m_context.ReadFloat();
        }

        public void Visit(SFVec2f field)
        {
            field.X = m_context.ReadFloat();
            field.Y = m_context.ReadFloat();
        }

        public void Visit(SFVec3f field)
        {
            field.X = m_context.ReadFloat();
            field.Y = m_context.ReadFloat();
            field.Z = m_context.ReadFloat();
        }


        public void Visit(SFRotation field)
        {
            field.X = m_context.ReadFloat();
            field.Y = m_context.ReadFloat();
            field.Z = m_context.ReadFloat();
            field.Angle = m_context.ReadFloat();
        }

        public void Visit(SFString field)
        {
            field.Value = m_context.ReadString();
        }

        public void Visit(SFColor field)
        {
            field.R = m_context.ReadFloat();
            field.G = m_context.ReadFloat();
            field.B = m_context.ReadFloat();
        }
        
        public void Visit(SFNode field)
        {
            VrmlToken token = m_context.PeekNextToken();
            switch (token.Text)
            {
                case "NULL":
                    field.Node = null;
                    break;
                default:
                    m_context.PushNodeContainer(field);
                    m_nodeStatementParser(m_context);
                    m_context.PopNodeContainer();
                    break;
            }
        }

        public void Visit(MFNode field)
        {
            field.ClearValues();
            m_context.PushNodeContainer(field);
            ParseMField(m_nodeStatementParser);
            m_context.PopNodeContainer();
        }

        public void Visit(MFVec2f field)
        {
            field.ClearValues();
            ParseMField((subcontext) =>
            {
                var child = new SFVec2f();
                Visit(child);
                field.AppendValue(child);
            });
        }

        public void Visit(MFVec3f field)
        {
            field.ClearValues();
            ParseMField((subcontext) =>
            {
                var child = new SFVec3f();
                Visit(child);
                field.AppendValue(child);
            });
        }

        public void Visit(MFInt32 field)
        {
            field.ClearValues();
            ParseMField((subcontext) =>
            {
                var child = new SFInt32();
                Visit(child);
                field.AppendValue(child);
            });
        }

        public void Visit(MFFloat field)
        {
            field.ClearValues();
            ParseMField((subcontext) =>
            {
                var child = new SFFloat();
                Visit(child);
                field.AppendValue(child);
            });
        }

        public void Visit(MFColor field)
        {
            field.ClearValues();
            ParseMField((subcontext) =>
            {
                var child = new SFColor();
                Visit(child);
                field.AppendValue(child);
            });
        }

        public void Visit(MFString field)
        {
            field.ClearValues();
            ParseMField(subcontext =>
            {
                var child = new SFString();
                Visit(child);
                field.AppendValue(child);
            });
        }

        public void Visit(MFRotation field)
        {
            field.ClearValues();
            ParseMField((subcontext) =>
            {
                var child = new SFRotation();
                Visit(child);
                field.AppendValue(child);
            });
        }

        protected virtual void ParseMField(Action<ParserContext> itemParser)
        {
            VrmlToken next = m_context.PeekNextToken();
            if (next.Type == VrmlTokenType.OpenBracket)
            {
                next = m_context.ReadNextToken();
                while (true)
                {
                    next = m_context.PeekNextToken();
                    if (next.Type == VrmlTokenType.CloseBracket) break;
                    itemParser(m_context);
                }
                next = m_context.ReadNextToken();
            }
            else
            {
                itemParser(m_context);
            }
        }
        
        public void Visit(SFImage field)
        {
            int width = m_context.ReadInt32();
            int height = m_context.ReadInt32();
            int components = m_context.ReadInt32();
            byte[,,] value = new byte[height, width, components];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    uint pixel = m_context.ReadHexadecimal();
                    switch (components)
                    {
                        case 1:
                            value[height - y - 1, x, 0] = (byte)(pixel & 0xff);
                            break;
                        case 2:
                            value[height - y - 1, x, 3] = (byte)(pixel & 0xff);
                            value[height - y - 1, x, 0] = (byte)((pixel >> 8) & 0xff);
                            break;
                        case 3:
                            value[height - y - 1, x, 2] = (byte)(pixel & 0xff);
                            value[height - y - 1, x, 1] = (byte)((pixel >> 8) & 0xff);
                            value[height - y - 1, x, 0] = (byte)((pixel >> 16) & 0xff);
                            break;
                        case 4:
                            value[height - y - 1, x, 3] = (byte)(pixel & 0xff);
                            value[height - y - 1, x, 2] = (byte)((pixel >> 8) & 0xff);
                            value[height - y - 1, x, 1] = (byte)((pixel >> 16) & 0xff);
                            value[height - y - 1, x, 0] = (byte)((pixel >> 24) & 0xff);
                            break;
                    }
                }
            }
            field.Value = value;
        }

        public void Visit(SFTime field)
        {
            field.Value = m_context.ReadDouble();
        }
    }
}
