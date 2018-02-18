using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class SpotLightNode : LightSourceNode
    {
        public SFVec3f  Attenuation => GetExposedField("attenuation")   as SFVec3f;
        public SFFloat  BeamWidth   => GetExposedField("beamWidth")     as SFFloat;
        public SFFloat  CutOffAngle => GetExposedField("cutOffAngle")   as SFFloat;
        public SFVec3f  Direction   => GetExposedField("direction")     as SFVec3f;
        public SFVec3f  Location    => GetExposedField("location")      as SFVec3f;
        public SFFloat  Radius      => GetExposedField("radius")        as SFFloat;

        public SpotLightNode()
        {
            AddExposedField("attenuation",  new SFVec3f(1, 0, 0));
            AddExposedField("beamWidth",    new SFFloat(1.570796f));
            AddExposedField("cutOffAngle",  new SFFloat(0.785398f));
            AddExposedField("direction",    new SFVec3f(0, 0, -1));
            AddExposedField("location",     new SFVec3f(0, 0, 0));
            AddExposedField("radius",       new SFFloat(100.0f));
        }

        protected override Node CreateInstance()
        {
            return new PointLightNode();
        }
    }
}
