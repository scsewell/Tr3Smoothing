using System;

namespace SoSmooth.Vrml.Tokenizer
{
    public class PunctuationState : VrmlTokenizerState
    {
        public PunctuationState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            char ch = m_context.ReadChar();
            if (m_tokenizer.IsOpenBrace(ch))
            {
                m_context.Enqueue(new VrmlToken(ch.ToString(), VrmlTokenType.OpenBrace));
                return new InitialState(m_context);
            }
            if (m_tokenizer.IsCloseBrace(ch))
            {
                m_context.Enqueue(new VrmlToken(ch.ToString(), VrmlTokenType.CloseBrace));
                return new InitialState(m_context);
            }
            if (m_tokenizer.IsOpenBracket(ch))
            {
                m_context.Enqueue(new VrmlToken(ch.ToString(), VrmlTokenType.OpenBracket));
                return new InitialState(m_context);
            }
            if (m_tokenizer.IsCloseBracket(ch))
            {
                m_context.Enqueue(new VrmlToken(ch.ToString(), VrmlTokenType.CloseBracket));
                return new InitialState(m_context);
            }
            throw new Exception("Unexpected character");
        }
    }
}
