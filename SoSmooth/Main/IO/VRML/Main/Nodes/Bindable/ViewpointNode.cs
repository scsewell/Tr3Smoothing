using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class ViewpointNode : BindableNode, IChildNode
    {
        public SFFloat      FieldOfView => GetExposedField("fieldOfView")   as SFFloat;
        public SFBool       Jump        => GetExposedField("jump")          as SFBool;
        public SFRotation   Orientation => GetExposedField("orientation")   as SFRotation;
        public SFVec3f      Position    => GetExposedField("position")      as SFVec3f;
        public SFString     Description => GetField("description")          as SFString;

        public ViewpointNode()
        {
            AddExposedField("fieldOfView",  new SFFloat(0.785398f));
            AddExposedField("jump",         new SFBool(true));
            AddExposedField("orientation",  new SFRotation(0, 0, 1, 0));
            AddExposedField("position",     new SFVec3f(0, 0, 10));
            AddField("description",         new SFString());
        }

        protected override Node CreateInstance()
        {
            return new ViewpointNode();
        }
    }
}
