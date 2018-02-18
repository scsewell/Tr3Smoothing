using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class PointLightNode : LightSourceNode
    {
        public SFVec3f Attenuation  => GetExposedField("attenuation")   as SFVec3f;
        public SFVec3f Location     => GetExposedField("location")      as SFVec3f;
        public SFFloat Radius       => GetExposedField("radius")        as SFFloat;

        public PointLightNode()
        {
            AddExposedField("attenuation",  new SFVec3f(1, 0, 0));
            AddExposedField("location",     new SFVec3f(0, 0, 0));
            AddExposedField("radius",       new SFFloat(100.0f));
        }

        protected override Node CreateInstance()
        {
            return new PointLightNode();
        }
    }
}
