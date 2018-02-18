using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
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
