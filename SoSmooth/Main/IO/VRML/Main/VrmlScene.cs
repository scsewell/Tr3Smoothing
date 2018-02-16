using SoSmooth.Vrml.Nodes;

namespace SoSmooth.Vrml
{
    public class VrmlScene
    {
        private readonly SceneGraphNode m_root;
        public SceneGraphNode Root => m_root;

        public VrmlScene()
        {
            m_root = new SceneGraphNode();
            m_root.Name = "SCENEGRAPH";
        }
    }
}
