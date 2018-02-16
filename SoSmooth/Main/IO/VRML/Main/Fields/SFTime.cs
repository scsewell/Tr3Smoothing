namespace SoSmooth.Vrml.Fields
{
    public class SFTime : Field
    {
        public double Value { get; set; }

        public override FieldType Type => FieldType.SFTime;

        public SFTime() : this(-1) { }

        public SFTime(double value)
        {
            Value = value;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFTime(Value);
        }
    }
}
