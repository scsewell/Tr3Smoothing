namespace SoSmooth.IO.Vrml.Fields
{
    public abstract class Field
    {
        public abstract FieldType Type { get; }
        
        public abstract void AcceptVisitor(IFieldVisitor visitor);

        public abstract Field Clone();

        public static Field CreateField(string fieldType)
        {
            switch (fieldType)
            {
                case "SFColor":
                    return new SFColor();
                case "SFFloat":
                    return new SFFloat();
                case "SFInt32":
                    return new SFInt32();
                default:
                    throw new InvalidVrmlSyntaxException("Unknown fieldType: '" + fieldType + "'");
            }
        }
    }
}
