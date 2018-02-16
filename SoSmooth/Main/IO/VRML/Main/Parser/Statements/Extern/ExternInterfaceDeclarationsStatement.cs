using System.Collections.Generic;

namespace SoSmooth.Vrml.Parser.Statements.Extern
{
    public class ExternInterfaceDeclarationsStatement
    {
        private readonly IList<ExternEventInStatement> m_eventIns = new List<ExternEventInStatement>();
        private readonly IList<ExternEventOutStatement> m_eventOuts = new List<ExternEventOutStatement>();
        private readonly IList<ExternFieldStatement> m_fields = new List<ExternFieldStatement>();
        private readonly IList<ExternExposedFieldStatement> m_exposedFields = new List<ExternExposedFieldStatement>();

        public IList<ExternEventInStatement> EventsIn => m_eventIns;
        public IList<ExternEventOutStatement> EventsOut => m_eventOuts;
        public IList<ExternFieldStatement> Fields => m_fields;
        public IList<ExternExposedFieldStatement> ExposedFields => m_exposedFields;

        public static ExternInterfaceDeclarationsStatement Parse(ParserContext context)
        {
            var res = new ExternInterfaceDeclarationsStatement();

            context.ReadOpenBracket();

            do
            {
                var token = context.PeekNextToken();
                if (token.Type == VrmlTokenType.CloseBracket)
                {
                    context.ReadCloseBracket();
                    break;
                }
                switch (token.Text)
                {
                    case "eventIn":
                        var eventIn = ExternEventInStatement.Parse(context);
                        res.EventsIn.Add(eventIn);
                        break;
                    case "eventOut":
                        var eventOut = ExternEventOutStatement.Parse(context);
                        res.EventsOut.Add(eventOut);
                        break;
                    case "field":
                        var field = ExternFieldStatement.Parse(context);
                        res.Fields.Add(field);
                        break;
                    case "exposedField":
                        var exposedField = ExternExposedFieldStatement.Parse(context);
                        res.ExposedFields.Add(exposedField);
                        break;
                    default:
                        throw new InvalidVrmlSyntaxException();
                }
            } while (true);

            return res;
        }
    }
}
