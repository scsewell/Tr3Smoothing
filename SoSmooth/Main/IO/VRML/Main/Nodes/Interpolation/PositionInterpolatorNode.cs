using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class PositionInterpolatorNode : InterpolatorNode<MFVec3f>
    {
        protected override Node CreateInstance()
        {
            return new PositionInterpolatorNode();
        }
    }
}
