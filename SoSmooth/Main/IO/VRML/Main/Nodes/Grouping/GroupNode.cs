namespace SoSmooth.IO.Vrml.Nodes
{
    public class GroupNode : GroupingNode, IChildNode
    {
        protected override Node CreateInstance()
        {
            return new GroupNode();
        }
    }
}
