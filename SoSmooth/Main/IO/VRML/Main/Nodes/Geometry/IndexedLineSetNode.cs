using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class IndexedLineSetNode : GeometryNode
    {
        public SFNode   Color           => GetExposedField("color")     as SFNode;
        public SFNode   Coord           => GetExposedField("coord")     as SFNode;
        public MFInt32  ColorIndex      => GetField("colorIndex")       as MFInt32;
        public SFBool   ColorPerVertex  => GetField("colorPerVertex")   as SFBool;
        public MFInt32  CoordIndex      => GetField("coordIndex")       as MFInt32;

        public IndexedLineSetNode()
        {
            AddExposedField("color",    new SFNode());
            AddExposedField("coord",    new SFNode());
            AddField("colorIndex",      new MFInt32());
            AddField("colorPerVertex",  new SFBool(true));
            AddField("coordIndex",      new MFInt32());
        }

        protected override Node CreateInstance()
        {
            return new IndexedLineSetNode();
        }
    }
}
