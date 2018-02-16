using System;
using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Parser.Statements.Proto
{
    public class ProtoExposedFieldStatement
    {
        public string FieldType { get; set; }
        public string FieldId { get; set; }
        public Field Value { get; set; }
        
        public static ProtoExposedFieldStatement Parse(ParserContext context, Action<ParserContext> nodeStatementParser)
        {
            context.ReadKeyword("exposedField");

            var fieldType = context.ParseFieldType();
            var fieldId = context.ParseFieldId();

            var field = context.CreateField(fieldType);
            var fieldParser = new FieldParser(context, nodeStatementParser);
            field.AcceptVisitor(fieldParser);

            return new ProtoExposedFieldStatement
            {
                FieldType = fieldType,
                FieldId = fieldId,
                Value = field
            };
        }
    }
}
