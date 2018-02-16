using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class ScriptNode : Node
    {
        public MFString Url             => GetExposedField("url")           as MFString;
        public SFBool   DirectOutput    => GetExposedField("directOutput")  as SFBool;
        public SFBool   MustEvaluate    => GetExposedField("mustEvaluate")  as SFBool;

        public ScriptNode()
        {
            AddExposedField("url", new MFString());
            AddField("directOutput", new SFBool(false));
            AddField("mustEvaluate", new SFBool(false));
        }

        protected override Node CreateInstance()
        {
            return new ScriptNode();
        }
    }
}
