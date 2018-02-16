using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class OrientationInterpolatorNode : InterpolatorNode<MFRotation>
    {
        protected override Node CreateInstance()
        {
            return new OrientationInterpolatorNode();
        }
    }
}
