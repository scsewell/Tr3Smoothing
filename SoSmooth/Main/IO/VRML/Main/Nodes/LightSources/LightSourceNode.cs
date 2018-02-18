using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public abstract class LightSourceNode : Node
    {
        public SFFloat  AmbientIntensity    => GetExposedField("ambientIntensity") as SFFloat;
        public SFFloat  Intensity           => GetExposedField("intensity") as SFFloat;
        public SFColor  Color               => GetExposedField("color")     as SFColor;
        public SFBool   On                  => GetExposedField("on")        as SFBool;

        public LightSourceNode()
        {
            AddExposedField("ambientIntensity", new SFFloat(0));
            AddExposedField("color",            new SFColor(1, 1, 1));
            AddExposedField("intensity",        new SFFloat(1));
            AddExposedField("on",               new SFBool(true));
        }
    }
}
