namespace SoSmooth.Vrml.Fields
{
    public class SFString : Field
    {
        public string Value { get; set; }

        public override FieldType Type => FieldType.SFString;

        public SFString() : this("") { }

        public SFString(string value)
        {
            Value = value;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFString(Value);
        }
        
        public static implicit operator string(SFString field)
        {
            return field.Value;
        }

        public static implicit operator SFString(string value)
        {
            return new SFString(value);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}
