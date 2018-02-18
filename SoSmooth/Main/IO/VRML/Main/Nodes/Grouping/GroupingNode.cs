using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public abstract class GroupingNode : Node
    {
        public MFNode   Children    => GetExposedField("children")  as MFNode;
        public SFVec3f  BboxCenter  => GetField("bboxCenter")       as SFVec3f;
        public SFVec3f  BboxSize    => GetField("bboxSize")         as SFVec3f;

        protected GroupingNode()
        {
            AddExposedField("children", new MFNode());
            AddField("bboxCenter", new SFVec3f(0, 0, 0));
            AddField("bboxSize", new SFVec3f(-1, -1, -1));
        }
    }
}
