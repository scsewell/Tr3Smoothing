using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class PositionInterpolatorNode : InterpolatorNode<MFVec3f>
    {
        protected override Node CreateInstance()
        {
            return new PositionInterpolatorNode();
        }
    }
}
