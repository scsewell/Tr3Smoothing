using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class CylinderNode : GeometryNode
    {
        public SFBool   Bottom  => GetField("bottom")   as SFBool;
        public SFFloat  Height  => GetField("height")   as SFFloat;
        public SFFloat  Radius  => GetField("radius")   as SFFloat;
        public SFBool   Side    => GetField("side")     as SFBool;
        public SFBool   Top     => GetField("top")      as SFBool;

        public CylinderNode()
        {
            AddField("bottom",  new SFBool(true));
            AddField("height",  new SFFloat(2));
            AddField("radius",  new SFFloat(1));
            AddField("side",    new SFBool(true));
            AddField("top",     new SFBool(true));
        }
        
        protected override Node CreateInstance()
        {
            return new CylinderNode();
        }
    }
}
