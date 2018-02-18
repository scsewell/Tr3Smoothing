using System;
using System.IO;

namespace SoSmooth.IO.Vrml.Tokenizer
{
    public class VrmlTokenizer
    {
        private readonly TokenizerContext m_context;
        private VrmlTokenizerState m_currentState;

        public int LineIndex => m_context.LineIndex;
        public int ColumnIndex => m_context.ColumnIndex;

        public VrmlTokenizer(Stream stream) : this(new StreamReader(stream))
        {
        }

        public VrmlTokenizer(TextReader reader)
        {
            m_context = new TokenizerContext(reader, this);
            m_currentState = new InitialState(m_context);
        }
        
        public VrmlToken ReadNextToken()
        {
            while (m_context.TokensCount == 0)
            {
                m_currentState = m_currentState.Tick();
            }
            return m_context.Dequeue();
        }

        public virtual bool IsEOF(char ch)
        {
            return ch == 0xffff;
        }

        public virtual bool IsWhiteSpace(char ch)
        {
            if (ch == 0x0d) return true;
            if (ch == 0x0a) return true;
            if (ch == 0x20) return true;
            if (ch == 0x09) return true;
            if (ch == 0x2c) return true;
            return false;
        }

        public virtual bool IsLineComment(char ch)
        {
            return ch == '#';
        }

        public virtual bool IsQuote(char ch)
        {
            return ch == '"';
        }

        public virtual bool IsEscapeSymbol(char ch)
        {
            return ch == '\\';
        }

        public bool IsIdFirstChar(char ch)
        {
            if (ch >= 0x0 && ch <= 0x20) return false;
            if (ch == 0x27) return false;
            if (ch == 0x7f) return false;
            if (IsEOF(ch)) return false;
            if (IsQuote(ch)) return false;
            if (IsLineComment(ch)) return false;
            if (IsPunctuation(ch)) return false;
            if (IsEscapeSymbol(ch)) return false;
            if (IsNumberFirstChar(ch)) return false;
            return true;
        }

        public bool IsIdRestChar(char ch)
        {
            if (ch >= 0x0 && ch <= 0x20) return false;
            if (ch == 0x27) return false;
            if (ch == 0x2b) return false;
            if (ch == 0x2e) return false;
            if (ch == 0x7f) return false;
            if (IsEOF(ch)) return false;
            if (IsPunctuation(ch)) return false;
            if (IsEscapeSymbol(ch)) return false;
            return true;
        }

        public bool IsNumberFirstChar(char ch)
        {
            if (ch >= 0x30 && ch <= 0x39) return true;
            if (ch == '-') return true;
            if (ch == '+') return true;
            if (ch == '.') return true;
            return false;
        }
        
        public virtual bool IsPunctuation(char ch)
        {
            if (IsOpenBrace(ch)) return true;
            if (IsCloseBrace(ch)) return true;
            if (IsOpenBracket(ch)) return true;
            if (IsCloseBracket(ch)) return true;
            if (ch == ',') return true;
            return false;
        }

        public bool IsOpenBrace(char ch)
        {
            return ch == '{';
        }

        public bool IsCloseBrace(char ch)
        {
            return ch == '}';
        }

        public bool IsOpenBracket(char ch)
        {
            return ch == '[';
        }

        public bool IsCloseBracket(char ch)
        {
            return ch == ']';
        }

        public bool IsMultipartIdentifierSeparator(char ch)
        {
            return ch == '.';
        }
    }
}
