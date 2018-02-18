using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class NormalNode : Node
    {
        public MFVec3f Vector => GetExposedField("vector") as MFVec3f;

        public NormalNode()
        {
            AddExposedField("vector", new MFVec3f());
        }

        protected override Node CreateInstance()
        {
            return new NormalNode();
        }
    }
}
