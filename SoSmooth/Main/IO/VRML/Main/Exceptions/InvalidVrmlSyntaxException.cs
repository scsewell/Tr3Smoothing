using System;

namespace SoSmooth.IO.Vrml
{
    public class InvalidVrmlSyntaxException : VrmlParseException
    {
        public InvalidVrmlSyntaxException() { }
        public InvalidVrmlSyntaxException(string message) : base(message) { }
        public InvalidVrmlSyntaxException(string message, Exception inner) : base(message, inner) { }
    }
}