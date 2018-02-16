using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class TimeSensorNode : SensorNode
    {
        public SFTime   CycleInterval   => GetExposedField("cycleInterval") as SFTime;
        public SFBool   Loop            => GetExposedField("loop")          as SFBool;
        public SFTime   StartTime       => GetExposedField("startTime")     as SFTime;
        public SFTime   StopTime        => GetExposedField("stopTime")      as SFTime;

        public TimeSensorNode()
        {
            AddExposedField("cycleInterval",    new SFTime(1));
            AddExposedField("loop",             new SFBool(false));
            AddExposedField("startTime",        new SFTime(0));
            AddExposedField("stopTime",         new SFTime(0));
        }

        protected override Node CreateInstance()
        {
            return new TimeSensorNode();
        }
    }
}
