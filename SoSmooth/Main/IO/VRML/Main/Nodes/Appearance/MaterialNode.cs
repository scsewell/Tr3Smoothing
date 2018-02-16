using SoSmooth.Vrml.Fields;

namespace SoSmooth.Vrml.Nodes
{
    public class MaterialNode : Node
    {
        public SFFloat SmbientIntensity => GetExposedField("ambientIntensity")  as SFFloat;
        public SFColor DiffuseColor     => GetExposedField("diffuseColor")      as SFColor;
        public SFColor EmissiveColor    => GetExposedField("emissiveColor")     as SFColor;
        public SFFloat Shininess        => GetExposedField("shininess")         as SFFloat;
        public SFColor SpecularColor    => GetExposedField("specularColor")     as SFColor;
        public SFFloat Transparency     => GetExposedField("transparency")      as SFFloat;

        public MaterialNode()
        {
            AddExposedField("ambientIntensity", new SFFloat(0.2f));
            AddExposedField("diffuseColor",     new SFColor(0.8f, 0.8f, 0.8f));
            AddExposedField("emissiveColor",    new SFColor(0, 0, 0));
            AddExposedField("shininess",        new SFFloat(0.2f));
            AddExposedField("specularColor",    new SFColor(0, 0, 0));
            AddExposedField("transparency",     new SFFloat(0));
        }
        
        protected override Node CreateInstance()
        {
            return new MaterialNode();
        }
    }
}
