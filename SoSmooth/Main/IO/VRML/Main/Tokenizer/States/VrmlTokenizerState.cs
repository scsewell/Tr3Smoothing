namespace SoSmooth.IO.Vrml.Tokenizer
{
    public abstract class VrmlTokenizerState
    {
        protected readonly VrmlTokenizer m_tokenizer;
        protected readonly TokenizerContext m_context;

        protected VrmlTokenizerState(TokenizerContext context)
        {
            m_context = context;
            m_tokenizer = context.Tokenizer;
        }

        public abstract VrmlTokenizerState Tick();
    }
}
