using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
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
