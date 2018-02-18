namespace SoSmooth.IO.Vrml.Fields
{
    public class MFString : MField<SFString>
    {
        public override FieldType Type => FieldType.MFString;
        
        public MFString(params SFString[] items) : base(items) { }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            var clone = new MFString();
            foreach (var child in Values)
            {
                clone.AppendValue((SFString)child.Clone());
            }
            return clone;
        }
    }
}

