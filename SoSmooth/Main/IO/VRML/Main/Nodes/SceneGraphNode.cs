namespace SoSmooth.IO.Vrml.Nodes
{
    public class SceneGraphNode : GroupingNode
    {
        protected override Node CreateInstance()
        {
            return new SceneGraphNode();
        }
    }
}
