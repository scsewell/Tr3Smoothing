using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class WorldInfoNode : Node, IChildNode
    {
        public MFString Info    => GetField("info") as MFString;
        public SFString Point   => GetField("title") as SFString;

        public WorldInfoNode()
        {
            AddField("info",    new MFString());
            AddField("title",   new SFString());
        }

        protected override Node CreateInstance()
        {
            return new WorldInfoNode();
        }
    }
}