using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public abstract class SensorNode : Node
    {
        public SFBool Enabled => GetExposedField("enabled") as SFBool;

        public SensorNode()
        {
            AddExposedField("enabled", new SFBool(true));
        }
    }
}
