using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class ScalarInterpolationNode : InterpolatorNode<MFFloat>
    {
        protected override Node CreateInstance()
        {
            return new ScalarInterpolationNode();
        }
    }
}
