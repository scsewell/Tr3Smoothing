namespace SoSmooth.Vrml.Fields
{
    public class SFVec2f : Field
    {
        public float X { get; set; }
        public float Y { get; set; }

        public override FieldType Type => FieldType.SFVec2f;

        public SFVec2f() : this(0, 0) { }

        public SFVec2f(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFVec2f(X, Y);
        }
        
        public override string ToString()
        {
            return $"{{x={X}, y={Y}}}";
        }
    }
}
