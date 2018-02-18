using System;

namespace SoSmooth.IO.Vrml.Tokenizer
{
    public class WordState : VrmlTokenizerState
    {
        private string m_text;

        public WordState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            char ch = m_context.PeekChar();
            if (string.IsNullOrEmpty(m_text))
            {
                if (m_tokenizer.IsIdFirstChar(ch))
                {
                    m_text += m_context.ReadChar();
                    return this;
                }
                else
                {
                    throw new Exception("Unexpected char");
                }
            }
            else
            {
                if (m_tokenizer.IsIdRestChar(ch))
                {
                    m_text += m_context.ReadChar();
                    return this;
                }
                else
                {
                    m_context.Enqueue(new VrmlToken(m_text, VrmlTokenType.Word));
                    if (m_tokenizer.IsMultipartIdentifierSeparator(ch))
                    {
                        return new MultipartIdentifierState(m_context);
                    }
                    else
                    {
                        return new InitialState(m_context);
                    }
                }
            }
        }
    }
}
