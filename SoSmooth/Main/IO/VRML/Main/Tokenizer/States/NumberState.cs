using System;

namespace SoSmooth.IO.Vrml.Tokenizer
{
    public class NumberState : VrmlTokenizerState
    {
        private string m_state = "";
        private string m_text = "";

        public NumberState(TokenizerContext context) : base(context)
        {
        }

        public override VrmlTokenizerState Tick()
        {
            //sndnesn - Sign, Number, Dot, Number, Exponenta, Sign, Number
            char ch = m_context.PeekChar();
            switch (m_state)
            {
                case "":
                    if (ch == '0')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "0";
                    }
                    else if (ch == '+' || ch == '-')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "s";
                    }
                    else if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sn";
                    }
                    else if (ch == '.')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "snd";
                    }
                    else
                    {
                        throw new Exception("Unexpected character");
                    }
                    break;
                case "0":
                    if (ch == 'x')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "0x";
                    }
                    else
                    {
                        m_state = "sn";
                    }
                    break;
                case "s":
                    if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sn";
                    }
                    else if (ch == '.')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "snd";
                    }
                    else
                    {
                        throw new Exception("Unexpected character");
                    }
                    break;
                case "sn":
                    if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sn";
                    }
                    else if (ch == '.')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "snd";
                    }
                    else if (m_tokenizer.IsWhiteSpace(ch) || m_tokenizer.IsPunctuation(ch))
                    {
                        m_context.Enqueue(new VrmlToken(m_text, VrmlTokenType.Word));
                        return new InitialState(m_context);
                    }
                    else
                    {
                        throw new TokenizerException("Invalid Number");
                    }
                    break;
                case "snd":
                    if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndn";
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndne";
                    }
                    else if (m_tokenizer.IsWhiteSpace(ch) || m_tokenizer.IsPunctuation(ch))
                    {
                        m_context.Enqueue(new VrmlToken(m_text, VrmlTokenType.Word));
                        return new InitialState(m_context);
                    }
                    else
                    {
                        throw new TokenizerException("Invalid Number");
                    }
                    break;
                case "sndn":
                    if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndn";
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndne";
                    }
                    else if (m_tokenizer.IsWhiteSpace(ch) || m_tokenizer.IsPunctuation(ch))
                    {
                        m_context.Enqueue(new VrmlToken(m_text, VrmlTokenType.Word));
                        return new InitialState(m_context);
                    }
                    else
                    {
                        throw new TokenizerException("Invalid Number");
                    }
                    break;
                case "sndne":
                    if (ch == '+' || ch == '-')
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndnes";
                    }
                    else if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndnesn";
                    }
                    else
                    {
                        throw new Exception("Unexpected character");
                    }
                    break;
                case "sndnes":
                    if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndnesn";
                    }
                    else
                    {
                        throw new Exception("Unexpected character");
                    }
                    break;
                case "sndnesn":
                    if (char.IsDigit(ch))
                    {
                        m_text += m_context.ReadChar();
                        m_state = "sndnesn";
                    }
                    else if (m_tokenizer.IsWhiteSpace(ch) || m_tokenizer.IsPunctuation(ch))
                    {
                        m_context.Enqueue(new VrmlToken(m_text, VrmlTokenType.Word));
                        return new InitialState(m_context);
                    }
                    else
                    {
                        throw new Exception("Unexpected character");
                    }
                    break;
            }
            return this;
        }
    }
}
