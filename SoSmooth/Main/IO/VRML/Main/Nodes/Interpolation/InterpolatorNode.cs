using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public abstract class InterpolatorNode<T> : InterpolatorNode where T : MField, new()
    {
        public T KeyValue => GetExposedField("keyValue") as T;

        public InterpolatorNode()
        {
            AddExposedField("keyValue", new T());
        }
    }

    public abstract class InterpolatorNode : Node
    {
        public MFFloat Key => GetExposedField("key") as MFFloat;

        protected InterpolatorNode()
        {
            AddExposedField("key", new MFFloat());
        }
    }
}
