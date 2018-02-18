using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class DirectionalLightNode : LightSourceNode
    {
        public SFVec3f Direction => GetExposedField("direction") as SFVec3f;

        public DirectionalLightNode()
        {
            AddExposedField("direction", new SFVec3f(0, 0, -1));
        }
        
        protected override Node CreateInstance()
        {
            return new DirectionalLightNode();
        }
    }
}
