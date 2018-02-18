using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class ScalarInterpolationNode : InterpolatorNode<MFFloat>
    {
        protected override Node CreateInstance()
        {
            return new ScalarInterpolationNode();
        }
    }
}
