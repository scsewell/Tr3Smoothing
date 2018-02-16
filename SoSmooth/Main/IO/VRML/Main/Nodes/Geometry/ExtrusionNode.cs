using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class ExtrusionNode : GeometryNode
    {
        public SFBool   BeginCap        => GetField("beginCap")     as SFBool;
        public SFBool   CCW             => GetField("ccwCap")       as SFBool;
        public SFBool   Convex          => GetField("convex")       as SFBool;
        public SFFloat  CreaseAngle     => GetField("creaseAngle")  as SFFloat;
        public MFVec2f  CrossSection    => GetField("crossSection") as MFVec2f;
        public SFBool   EndCap          => GetField("endCap")       as SFBool;
        public MFVec2f  Scale           => GetField("scale")        as MFVec2f;
        public MFVec3f  Spine           => GetField("spine")        as MFVec3f;

        public ExtrusionNode()
        {
            AddField("beginCap",        new SFBool(true));
            AddField("ccwCap",          new SFBool(true));
            AddField("convex",          new SFBool(true));
            AddField("creaseAngle",     new SFFloat(0));
            AddField("crossSection",    new MFVec2f(
                                            new SFVec2f(1, 1),
                                            new SFVec2f(1, -1),
                                            new SFVec2f(-1, -1),
                                            new SFVec2f(-1, 1),
                                            new SFVec2f(1, 1)
                                        ));
            AddField("endCap",          new SFBool(true));
            AddField("scale",           new MFVec2f(new SFVec2f(1, 1)));
            AddField("spine",           new MFVec3f(
                                            new SFVec3f(0, 0, 0),
                                            new SFVec3f(0, 1, 0)
                                        ));
        }

        protected override Node CreateInstance()
        {
            return new ExtrusionNode();
        }
    }
}
