using System;

namespace SoSmooth.Vrml.Tokenizer
{
    public class VrmlToken
    {
        private readonly string m_text;
        public string Text => m_text;

        private readonly VrmlTokenType m_type;
        public VrmlTokenType Type => m_type;

        public VrmlToken(string text, VrmlTokenType type)
        {
            m_text = text;
            m_type = type;
        }

        public override string ToString()
        {
            switch (m_type)
            {
                case VrmlTokenType.Word:
                    return "Token: " + m_text;
                case VrmlTokenType.EOF:
                    return "EOF Token";
                case VrmlTokenType.OpenBrace:
                    return "OpenBrace ('{') Token";
                case VrmlTokenType.CloseBrace:
                    return "CloseBrace ('}') Token";
                case VrmlTokenType.OpenBracket:
                    return "OpenBracket ('[') Token";
                case VrmlTokenType.CloseBracket:
                    return "CloseBracket (']') Token";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
