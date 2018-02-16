using System;
using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Parser.Statements
{
    public class ExposedFieldStatement
    {
        public string FieldType { get; set; }
        public string FieldId { get; set; }
        public Field Value { get; set; }
        
        public static ExposedFieldStatement Parse(ParserContext context, Action<ParserContext> nodeStatementParser)
        {
            context.ReadKeyword("exposedField");

            var fieldType = context.ParseFieldType();
            var fieldId = context.ParseFieldId();
            
            var field = context.CreateField(fieldType);
            var fieldParser = new FieldParser(context, nodeStatementParser);
            field.AcceptVisitor(fieldParser);

            return new ExposedFieldStatement
            {
                FieldType = fieldType,
                FieldId = fieldId,
                Value = field
            };
        }
    }
}
