using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class CoordinateNode : Node
    {
        public MFVec3f Point => GetExposedField("point") as MFVec3f;

        public CoordinateNode()
        {
            AddExposedField("point", new MFVec3f());
        }

        protected override Node CreateInstance()
        {
            return new CoordinateNode();
        }
    }
}
