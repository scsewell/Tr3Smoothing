namespace SoSmooth.IO.Vrml.Fields
{
    public class MFColor : MField<SFColor>
    {
        public override FieldType Type => FieldType.MFColor;

        public MFColor() { }
        public MFColor(params SFColor[] items) : base(items) { }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            var clone = new MFColor();
            foreach (var child in Values)
            {
                clone.AppendValue((SFColor)child.Clone());
            }
            return clone;
        }
    }
}
