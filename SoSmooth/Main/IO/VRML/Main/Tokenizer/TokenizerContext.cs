using System;
using System.Collections.Generic;
using System.IO;

namespace SoSmooth.Vrml.Tokenizer
{
    public class TokenizerContext
    {
        private readonly TextReader m_reader;
        private readonly VrmlTokenizer m_tokenizer;
        private readonly Queue<VrmlToken> m_tokens = new Queue<VrmlToken>();
        private int m_lineIndex = 1;
        private int m_columnIndex = 1;
        private string state = "";

        public VrmlTokenizer Tokenizer => m_tokenizer;
        public int LineIndex => m_lineIndex;
        public int ColumnIndex => m_columnIndex;

        public TokenizerContext(TextReader reader, VrmlTokenizer tokenizer)
        {
            m_reader = reader;
            m_tokenizer = tokenizer;
        }
        
        public void Enqueue(VrmlToken token)
        {
            m_tokens.Enqueue(token);
        }
        
        public VrmlToken Dequeue()
        {
            return m_tokens.Dequeue();
        }

        public int TokensCount
        {
            get { return m_tokens.Count; }
        }
        
        public char PeekChar()
        {
            return (char)m_reader.Peek();
        }
        
        public string ReadLine()
        {
            string line = m_reader.ReadLine();
            m_lineIndex++;
            m_columnIndex = 1;
            return line;
        }
        
        public char ReadChar()
        {
            char ch = (char)m_reader.Read();
            switch (state)
            {
                case "":
                    switch (ch)
                    {
                        case '\r':
                            state = "r";
                            break;
                        case '\n':
                            m_lineIndex++;
                            m_columnIndex = 1;
                            break;
                        default:
                            m_columnIndex++;
                            break;
                    }
                    break;
                case "r":
                    if (ch == '\n')
                    {
                        m_lineIndex++;
                        m_columnIndex = 1;
                    }
                    state = "";
                    break;
            }
            return ch;
        }
    }
}
