using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class TextureCoordinateNode : Node
    {
        public MFVec2f Point => GetExposedField("point") as MFVec2f;

        public TextureCoordinateNode()
        {
            AddExposedField("point", new MFVec2f());
        }
        
        protected override Node CreateInstance()
        {
            return new TextureCoordinateNode();
        }
    }
}
