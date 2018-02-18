using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class TextNode : GeometryNode
    {
        public MFString String      => GetExposedField("string")    as MFString;
        public SFNode   FontStyle   => GetExposedField("fontStyle") as SFNode;
        public MFFloat  Length      => GetExposedField("length")    as MFFloat;
        public SFFloat  MaxExtent   => GetExposedField("maxExtent") as SFFloat;

        public TextNode()
        {
            AddExposedField("string",       new MFString());
            AddExposedField("fontStyle",    new SFNode());
            AddExposedField("length",       new MFFloat());
            AddExposedField("maxExtent",    new SFFloat(0.0f));
        }
        
        protected override Node CreateInstance()
        {
            return new TextNode();
        }
    }
}
