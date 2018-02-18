using System;

namespace SoSmooth.IO.Vrml
{
    public class VrmlParseException : Exception
    {
        public VrmlParseException() { }
        public VrmlParseException(string message) : base(message) { }
        public VrmlParseException(string message, Exception inner) : base(message, inner) { }
    }
}
