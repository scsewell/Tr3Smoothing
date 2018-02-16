using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class ConeNode : GeometryNode
    {
        public SFFloat  BottomRadius    => GetField("bottomRadius") as SFFloat;
        public SFFloat  Height          => GetField("height")       as SFFloat;
        public SFBool   Side            => GetField("side")         as SFBool;
        public SFBool   Bottom          => GetField("bottom")       as SFBool;

        public ConeNode()
        {
            AddField("bottomRadius",    new SFFloat(1));
            AddField("height",          new SFFloat(2));
            AddField("side",            new SFBool(true));
            AddField("bottom",          new SFBool(true));
        }
        
        protected override Node CreateInstance()
        {
            return new ConeNode();
        }
    }
}
