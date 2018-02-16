using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class TransformNode : GroupingNode, IChildNode
    {
        public SFVec3f      Center      => GetExposedField("center")        as SFVec3f;
        public SFRotation   Rotation    => GetExposedField("rotation")      as SFRotation;
        public SFVec3f      Scale       => GetExposedField("scale")         as SFVec3f;
        public SFRotation   ScaleOrientation => GetExposedField("scaleOrientation") as SFRotation;
        public SFVec3f      Translation => GetExposedField("translation")   as SFVec3f;

        public TransformNode()
        {
            AddExposedField("center",           new SFVec3f(0, 0, 0));
            AddExposedField("rotation",         new SFRotation(0, 0, 1, 0));
            AddExposedField("scale",            new SFVec3f(1, 1, 1));
            AddExposedField("scaleOrientation", new SFRotation(0, 0, 1, 0));
            AddExposedField("translation",      new SFVec3f(0, 0, 0));
        }

        protected override Node CreateInstance()
        {
            return new TransformNode();
        }
    }
}
