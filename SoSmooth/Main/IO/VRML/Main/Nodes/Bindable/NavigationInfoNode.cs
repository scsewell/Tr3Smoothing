using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class NavigationInfoNode : BindableNode, IChildNode
    {
        public MFFloat  AvatarSize      => GetExposedField("avatarSize")    as MFFloat;
        public SFBool   Headlight       => GetExposedField("headlight")     as SFBool;
        public SFFloat  Speed           => GetExposedField("speed")         as SFFloat;
        public MFString Type            => GetExposedField("type")          as MFString;
        public SFFloat  VisibilityLimit => GetExposedField("visibilityLimit") as SFFloat;

        public NavigationInfoNode()
        {
            AddExposedField("avatarSize",       new MFFloat(
                                                    new SFFloat(0.25f), 
                                                    new SFFloat(1.6f), 
                                                    new SFFloat(0.75f)
                                                ));
            AddExposedField("headlight",        new SFBool(true));
            AddExposedField("speed",            new SFFloat(1.0f));
            AddExposedField("type",             new MFString("WALK", "ANY"));
            AddExposedField("visibilityLimit",  new SFFloat(0.0f));
        }

        protected override Node CreateInstance()
        {
            return new NavigationInfoNode();
        }
    }
}
