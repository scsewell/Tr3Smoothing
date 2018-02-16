using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class CoordinateInterpolatorNode : InterpolatorNode<MFVec3f>
    {
        public CoordinateInterpolatorNode()
        {
        }

        protected override Node CreateInstance()
        {
            return new CoordinateInterpolatorNode();
        }
    }
}
