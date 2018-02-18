using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class AppearanceNode : Node
    {
        public SFNode   Material            => GetExposedField("material")          as SFNode;
        public SFNode   Texture             => GetExposedField("texture")           as SFNode;
        public SFNode   TextureTransform    => GetExposedField("textureTransform")  as SFNode;

        public AppearanceNode()
        {
            AddExposedField("material",         new SFNode());
            AddExposedField("texture",          new SFNode());
            AddExposedField("textureTransform", new SFNode());
        }

        protected override Node CreateInstance()
        {
            return new AppearanceNode();
        }
    }
}
