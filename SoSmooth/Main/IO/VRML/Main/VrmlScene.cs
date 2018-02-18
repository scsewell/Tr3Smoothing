using SoSmooth.IO.Vrml.Nodes;

namespace SoSmooth.IO.Vrml
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
