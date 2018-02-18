namespace SoSmooth.IO.Vrml.Tokenizer
{
    public class LineCommentState : VrmlTokenizerState
    {
        protected string m_text = string.Empty;
        private bool m_reachedEOL = false;

        public LineCommentState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            char ch = m_context.ReadChar();
            switch (ch)
            {
                case '\r':
                case '\n':
                    m_reachedEOL = true;
                    break;
                default:
                    m_text += ch;
                    break;
            }
            if (m_tokenizer.IsEOF(ch))
            {
                m_reachedEOL = true;
            }
            if (m_reachedEOL)
            {
                return new InitialState(m_context);
            }
            else
            {
                return this;
            }
        }
    }
}
