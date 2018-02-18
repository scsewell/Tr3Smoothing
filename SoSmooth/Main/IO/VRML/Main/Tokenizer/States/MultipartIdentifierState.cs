using System;

namespace SoSmooth.IO.Vrml.Tokenizer
{
    public class MultipartIdentifierState : VrmlTokenizerState
    {
        public MultipartIdentifierState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            char ch = m_context.PeekChar();
            if (m_tokenizer.IsMultipartIdentifierSeparator(ch))
            {
                m_context.Enqueue(new VrmlToken(m_context.ReadChar().ToString(), VrmlTokenType.MutipartIdentifierSeparator));
                return new WordState(m_context);
            }
            else
            {
                throw new Exception("Unexpected context");
            }
        }
    }
}
