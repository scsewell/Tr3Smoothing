using SoSmooth.IO.Vrml.Nodes;

namespace SoSmooth.IO.Vrml.Fields
{
    public class MFNode : MField<Node>
    {
        public override FieldType Type => FieldType.MFNode;

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            var clone = new MFNode();
            foreach (var child in Values)
            {
                clone.AppendValue(child.Clone());
            }
            return clone;
        }
    }
}
