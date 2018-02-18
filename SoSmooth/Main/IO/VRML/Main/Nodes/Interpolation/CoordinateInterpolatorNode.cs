using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
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
