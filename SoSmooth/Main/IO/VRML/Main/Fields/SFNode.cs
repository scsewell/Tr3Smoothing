using SoSmooth.IO.Vrml.Nodes;

namespace SoSmooth.IO.Vrml.Fields
{
    public class SFNode : Field
    {
        public Node Node { get; set; }

        public override FieldType Type => FieldType.SFNode;

        public SFNode() { }

        public SFNode(Node value)
        {
            Node = value;
        }

        public override void AcceptVisitor(IFieldVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Field Clone()
        {
            return new SFNode(Node != null ? Node.Clone() : null);
        }
        
        public override string ToString()
        {
            return $"[{Node}]";
        }
    }
}
