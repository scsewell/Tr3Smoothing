using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class OrientationInterpolatorNode : InterpolatorNode<MFRotation>
    {
        protected override Node CreateInstance()
        {
            return new OrientationInterpolatorNode();
        }
    }
}
