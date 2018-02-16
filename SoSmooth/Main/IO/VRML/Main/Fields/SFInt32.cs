namespace SoSmooth.Vrml.Fields
{
    public class SFInt32 : Field
    {
        public int Value { get; set; }

        public override FieldType Type => FieldType.SFInt32;

        public SFInt32() : this(0) { }

        public SFInt32(int value)
        {
            Value = value;
        }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public static implicit operator int(SFInt32 field)
        {
            return field.Value;
        }

        public override Field Clone()
        {
            return new SFInt32(Value);
        }
    }
}
