namespace SoSmooth.IO.Vrml.Fields
{
    public class SFColor : Field
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public override FieldType Type => FieldType.SFColor;

        public SFColor() : this(0, 0, 0) { }

        public SFColor(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }
        
        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFColor(R, G, B);
        }
        
        public override string ToString()
        {
            return $"{{r={R}, g={G}, b={B}}}";
        }
    }
}
