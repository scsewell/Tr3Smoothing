using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class PixelTextureNode : TextureNode
    {
        public SFImage Image => GetExposedField("image") as SFImage;

        public PixelTextureNode()
        {
            AddExposedField("image", new SFImage());
        }
        
        protected override Node CreateInstance()
        {
            return new PixelTextureNode();
        }
    }
}
