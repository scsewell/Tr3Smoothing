using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class BillboardNode : GroupingNode, IChildNode
    {
        public SFVec3f AxisOfRotation => GetExposedField("axisOfRotation") as SFVec3f;

        public BillboardNode()
        {
            AddExposedField("axisOfRotation", new SFVec3f(0, 1, 0));
        }

        protected override Node CreateInstance()
        {
            return new BillboardNode();
        }
    }
}
