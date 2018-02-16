namespace SoSmooth.Vrml.Fields
{
    public class SFBool : Field
    {
        public bool Value { get; set; }

        public override FieldType Type => FieldType.SFBool;

        public SFBool() : this(false) { }

        public SFBool(bool value)
        {
            Value = value;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFBool(Value);
        }

        public static implicit operator bool(SFBool field)
        {
            return field.Value;
        }

        public override string ToString()
        {
            return Value ? "TRUE" : "FALSE";
        }
    }
}
