using System;

namespace SoSmooth.Vrml.Tokenizer
{
    public class StringConstantState : VrmlTokenizerState
    {
        private string m_state = "";
        private string m_text = "";
        private char m_quote;

        public StringConstantState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            char ch = m_context.ReadChar();
            switch (m_state)
            {
                case "":
                    if (m_tokenizer.IsQuote(ch))
                    {
                        m_quote = ch;
                        m_state = "q";
                    }
                    else
                    {
                        throw new Exception("Unexpected state");
                    }
                    break;
                case "q":
                    if (m_tokenizer.IsQuote(ch))
                    {
                        m_context.Enqueue(new VrmlToken(m_text, VrmlTokenType.Word));
                        return new InitialState(m_context);
                    }
                    if (m_tokenizer.IsEscapeSymbol(ch))
                    {
                        m_state = "e";
                        break;
                    }
                    m_text += ch;
                    break;
                case "e":
                    if (ch == m_quote)
                    {
                        m_text += ch;
                        m_state = "q";
                        break;
                    }
                    if (m_tokenizer.IsEscapeSymbol(ch))
                    {
                        m_text += ch;
                        m_state = "q";
                        break;
                    }
                    throw new InvalidVrmlSyntaxException("Unexpected escape symbol " + ch.ToString());
            }
            return this;
        }

    }

}
