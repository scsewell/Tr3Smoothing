using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
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
