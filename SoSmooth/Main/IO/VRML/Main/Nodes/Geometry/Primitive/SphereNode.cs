using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class SphereNode : GeometryNode
    {
        public SFFloat Radius => GetField("radius") as SFFloat;

        public SphereNode()
        {
            AddField("radius", new SFFloat(1));
        }

        protected override Node CreateInstance()
        {
            return new SphereNode();
        }
    }
}
