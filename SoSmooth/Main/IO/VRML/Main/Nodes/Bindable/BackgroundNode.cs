using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class BackgroundNode : BindableNode
    {
        public MFColor  GroundColor => GetExposedField("groundColor")   as MFColor;
        public MFFloat  GroundAngle => GetExposedField("groundAngle")   as MFFloat;
        public MFString BackUrl     => GetExposedField("backUrl")       as MFString;
        public MFString BottomUrl   => GetExposedField("bottomUrl")     as MFString;
        public MFString FrontUrl    => GetExposedField("frontUrl")      as MFString;
        public MFString LeftUrl     => GetExposedField("leftUrl")       as MFString;
        public MFString RightUrl    => GetExposedField("rightUrl")      as MFString;
        public MFString TopUrl      => GetExposedField("topUrl")        as MFString;
        public MFColor  SkyColor    => GetExposedField("skyColor")      as MFColor;
        public MFFloat  SkyAngle    => GetExposedField("skyAngle")      as MFFloat;
        
        public BackgroundNode()
        {
            AddExposedField("groundColor",  new MFColor());
            AddExposedField("groundAngle",  new MFFloat());
            AddExposedField("backUrl",      new MFString());
            AddExposedField("bottomUrl",    new MFString());
            AddExposedField("frontUrl",     new MFString());
            AddExposedField("leftUrl",      new MFString());
            AddExposedField("rightUrl",     new MFString());
            AddExposedField("topUrl",       new MFString());
            AddExposedField("skyAngle",     new MFFloat());
            AddExposedField("skyColor",     new MFColor());
        }

        protected override Node CreateInstance()
        {
            return new BackgroundNode();
        }
    }
}
