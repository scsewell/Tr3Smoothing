namespace SoSmooth.Vrml.Fields
{
    public class MFFloat : MField<SFFloat>
    {
        public override FieldType Type => FieldType.MFFloat;

        public MFFloat() { }
        public MFFloat(params SFFloat[] items) : base(items) { }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            var clone = new MFFloat();
            foreach (var child in Values)
            {
                clone.AppendValue((SFFloat)child.Clone());
            }
            return clone;
        }
    }
}
