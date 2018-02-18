using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class IndexedFaceSetNode : GeometryNode
    {
        public SFNode   Color       => GetExposedField("color")     as SFNode;
        public SFNode   Coord       => GetExposedField("coord")     as SFNode;
        public SFNode   Normal      => GetExposedField("normal")    as SFNode;
        public SFNode   TexCoord    => GetExposedField("texCoord")  as SFNode;

        public SFBool   CCW                 => GetField("ccw")              as SFBool;
        public MFInt32  ColorIndex          => GetField("colorIndex")       as MFInt32;
        public SFBool   ColorPerVertex      => GetField("colorPerVertex")   as SFBool;
        public SFBool   Convex              => GetField("convex")           as SFBool;
        public MFInt32  CoordIndex          => GetField("coordIndex")       as MFInt32;
        public SFFloat  CreaseAngle         => GetField("creaseAngle")      as SFFloat;
        public MFInt32  NormalIndex         => GetField("normalIndex")      as MFInt32;
        public SFBool   NormalPerVertex     => GetField("normalPerVertex")  as SFBool;
        public SFBool   Solid               => GetField("solid")            as SFBool;
        public MFInt32  TexCoordIndex       => GetField("texCoordIndex")    as MFInt32;
        
        public IndexedFaceSetNode()
        {
            AddExposedField("color",    new SFNode());
            AddExposedField("coord",    new SFNode());
            AddExposedField("normal",   new SFNode());
            AddExposedField("texCoord", new SFNode());

            AddField("ccw",             new SFBool(true));
            AddField("colorIndex",      new MFInt32());
            AddField("colorPerVertex",  new SFBool(true));
            AddField("convex",          new SFBool(true));
            AddField("coordIndex",      new MFInt32());
            AddField("creaseAngle",     new SFFloat(0));
            AddField("normalIndex",     new MFInt32());
            AddField("normalPerVertex", new SFBool(true));
            AddField("solid",           new SFBool(true));
            AddField("texCoordIndex",   new MFInt32());
        }

        protected override Node CreateInstance()
        {
            return new IndexedFaceSetNode();
        }
    }
}
