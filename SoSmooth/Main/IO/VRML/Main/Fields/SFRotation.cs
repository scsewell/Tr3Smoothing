namespace SoSmooth.Vrml.Fields
{
    public class SFRotation : Field
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }

        public override FieldType Type => FieldType.SFRotation;

        public SFRotation() : this(0, 0, 1, 0) { }

        public SFRotation(float x, float y, float z, float angle)
        {
            X = x;
            Y = y;
            Z = z;
            Angle = angle;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFRotation(X, Y, Z, Angle);
        }
        
        public override string ToString()
        {
            return $"{{x={X}, y={Y}, z={Z}, angle={Angle}}}";
        }
    }
}
