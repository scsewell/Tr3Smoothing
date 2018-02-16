using System;
using System.Collections.Generic;

namespace SoSmooth.Vrml.Parser.Statements.Proto
{
    public class ProtoInterfaceDeclarationsStatement
    {
        private readonly IList<ProtoEventInStatement> m_eventIns = new List<ProtoEventInStatement>();
        private readonly IList<ProtoEventOutStatement> m_eventOuts = new List<ProtoEventOutStatement>();
        private readonly IList<ProtoFieldStatement> m_fields = new List<ProtoFieldStatement>();
        private readonly IList<ProtoExposedFieldStatement> m_exposedFields = new List<ProtoExposedFieldStatement>();

        public IList<ProtoEventInStatement> EventsIn => m_eventIns;
        public IList<ProtoEventOutStatement> EventsOut => m_eventOuts;
        public IList<ProtoFieldStatement> Fields => m_fields;
        public IList<ProtoExposedFieldStatement> ExposedFields => m_exposedFields;
        
        public static ProtoInterfaceDeclarationsStatement Parse(ParserContext context, Action<ParserContext> nodeStatementParser)
        {
            var res = new ProtoInterfaceDeclarationsStatement();

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
                        var eventIn = ProtoEventInStatement.Parse(context);
                        res.EventsIn.Add(eventIn);
                        break;
                    case "eventOut":
                        var eventOut = ProtoEventOutStatement.Parse(context);
                        res.EventsOut.Add(eventOut);
                        break;
                    case "field":
                        var field = ProtoFieldStatement.Parse(context, nodeStatementParser);
                        res.Fields.Add(field);
                        break;
                    case "exposedField":
                        var exposedField = ProtoExposedFieldStatement.Parse(context, nodeStatementParser);
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
