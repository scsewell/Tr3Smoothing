using System;

namespace SoSmooth.Vrml.Tokenizer
{
    public class InitialState : VrmlTokenizerState
    {
        public InitialState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            char ch = m_context.PeekChar();
            if (m_tokenizer.IsWhiteSpace(ch))
            {
                m_context.ReadChar();
                return this;
            }
            if (m_tokenizer.IsEOF(ch))
            {
                m_context.Enqueue(new VrmlToken("", VrmlTokenType.EOF));
                return this;
            }
            if (m_tokenizer.IsLineComment(ch))
            {
                return new LineCommentState(m_context);
            }
            if (m_tokenizer.IsPunctuation(ch))
            {
                return new PunctuationState(m_context);
            }
            if (m_tokenizer.IsQuote(ch))
            {
                return new StringConstantState(m_context);
            }
            if (m_tokenizer.IsIdFirstChar(ch))
            {
                return new WordState(m_context);
            }
            if (m_tokenizer.IsNumberFirstChar(ch))
            {
                return new NumberState(m_context);
            }
            throw new Exception("Unexpected symbol");
        }
    }

}
