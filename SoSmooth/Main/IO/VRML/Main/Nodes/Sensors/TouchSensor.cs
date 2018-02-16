namespace SoSmooth.Vrml.Nodes
{
    public class TouchSensorNode : SensorNode
    {
        protected override Node CreateInstance()
        {
            return new TimeSensorNode();
        }
    }
}
