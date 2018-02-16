namespace SoSmooth.Vrml.Fields
{
    public class SFImage : Field
    {
        public byte[,,] Value { get; set; }

        public int Width => Value.GetLength(1);
        public int Height => Value.GetLength(0);
        public int Components => Value.GetLength(2);

        public override FieldType Type => FieldType.SFImage;

        public SFImage()
        {
            Value = new byte[0, 0, 0];
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            SFImage clone = new SFImage { Value = (byte[,,])Value.Clone() };
            return clone;
        }
    }
}
