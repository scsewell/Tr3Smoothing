using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class FontStyleNode : Node, IChildNode
    {
        public MFString Family          => GetField("family")       as MFString;
        public SFBool   Horizontal      => GetField("horizontal")   as SFBool;
        public MFString Justify         => GetField("justify")      as MFString;
        public SFString Language        => GetField("language")     as SFString;
        public SFBool   LeftToRight     => GetField("leftToRight")  as SFBool;
        public SFFloat  Size            => GetField("size")         as SFFloat;
        public SFFloat  Spacing         => GetField("spacing")      as SFFloat;
        public SFString Style           => GetField("style")        as SFString;
        public SFBool   TopToBottom     => GetField("topToBottom")  as SFBool;

        public FontStyleNode()
        {
            AddField("family",      new MFString("SERIF"));
            AddField("horizontal",  new SFBool(true));
            AddField("justify",     new MFString("BEGIN"));
            AddField("language",    new SFString(""));
            AddField("leftToRight", new SFBool(true));
            AddField("size",        new SFFloat(1.0f));
            AddField("spacing",     new SFFloat(1.0f));
            AddField("style",       new SFString("PLAIN"));
            AddField("topToBottom", new SFBool(true));
        }

        protected override Node CreateInstance()
        {
            return new FontStyleNode();
        }
    }
}
