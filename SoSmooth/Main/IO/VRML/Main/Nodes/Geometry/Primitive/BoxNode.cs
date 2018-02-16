using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class BoxNode : GeometryNode
    {
        public SFVec3f Size => GetField("size") as SFVec3f;

        public BoxNode()
        {
            AddField("size", new SFVec3f(2, 2, 2));
        }

        protected override Node CreateInstance()
        {
            return new BoxNode();
        }
    }
}
