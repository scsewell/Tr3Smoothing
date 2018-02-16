using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class ColorInterpolatorNode : InterpolatorNode<MFColor>
    {
        public ColorInterpolatorNode()
        {
        }

        protected override Node CreateInstance()
        {
            return new ColorInterpolatorNode();
        }
    }
}
