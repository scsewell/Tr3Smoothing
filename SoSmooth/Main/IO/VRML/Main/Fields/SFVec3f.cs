namespace SoSmooth.Vrml.Fields
{
    public class SFVec3f : Field
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override FieldType Type => FieldType.SFVec3f;

        public SFVec3f() : this(0, 0, 0) { }

        public SFVec3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFVec3f(X, Y, Z);
        }
        
        public override string ToString()
        {
            return $"{{x={X}, y={Y}, z={Z}}}";
        }
    }
}
