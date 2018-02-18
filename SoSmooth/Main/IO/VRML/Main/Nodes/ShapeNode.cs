using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class ShapeNode : Node, IChildNode
    {
        public SFNode   Appearance  => GetExposedField("appearance")    as SFNode;
        public SFNode   Geometry    => GetExposedField("geometry")      as SFNode;

        public ShapeNode()
        {
            AddExposedField("appearance", new SFNode());
            AddExposedField("geometry", new SFNode());
        }

        protected override Node CreateInstance()
        {
            return new ShapeNode();
        }
    }
}
