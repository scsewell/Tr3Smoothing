using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public class ProtoNode : GroupingNode
    {
        public new void AddField(string fieldName, Field field)
        {
            base.AddField(fieldName, field);
        }

        public new void AddExposedField(string exposedFieldName, Field field)
        {
            base.AddExposedField(exposedFieldName, field);
        }

        protected override Node CreateInstance()
        {
            return new ProtoNode();
        }
    }
}
