using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class AnchorNode : GroupingNode, IChildNode
    {
        public SFString Description => GetExposedField("description") as SFString;
        public MFString Parameter   => GetField("parameter") as MFString;
        public MFString Url         => GetField("url") as MFString;

        public AnchorNode()
        {
            AddExposedField("description",  new SFString());
            AddField("parameter",    new MFString());
            AddField("url",          new MFString());
        }

        protected override Node CreateInstance()
        {
            return new AnchorNode();
        }
    }
}
