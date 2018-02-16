namespace SoSmooth.Vrml.Fields
{
    public class MFInt32 : MField<SFInt32>
    {
        public override FieldType Type => FieldType.MFInt32;

        public MFInt32(params SFInt32[] items) : base(items) { }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            var clone = new MFInt32();
            foreach (var child in Values)
            {
                clone.AppendValue((SFInt32)child.Clone());
            }
            return clone;
        }
    }
}
