using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
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
