using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class ColorNode : Node
    {
        public MFColor Color => GetExposedField("color") as MFColor;

        public ColorNode()
        {
            AddExposedField("color", new MFColor());
        }

        protected override Node CreateInstance()
        {
            return new ColorNode();
        }
    }
}
