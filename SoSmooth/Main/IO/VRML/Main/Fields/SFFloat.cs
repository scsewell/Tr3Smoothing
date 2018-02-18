namespace SoSmooth.IO.Vrml.Fields
{
    public class SFFloat : Field
    {
        public float Value { get; set; }

        public override FieldType Type => FieldType.SFFloat;

        public SFFloat() : this(0) { }

        public SFFloat(float value)
        {
            Value = value;
        }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFFloat(Value);
        }

        public static implicit operator SFFloat(float value)
        {
            return new SFFloat(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
