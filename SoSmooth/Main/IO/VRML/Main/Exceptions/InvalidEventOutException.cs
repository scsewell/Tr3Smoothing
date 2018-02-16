using System;

namespace SoSmooth.Vrml
{
    public class InvalidEventOutException : VrmlParseException
    {
        public InvalidEventOutException() { }
        public InvalidEventOutException(string message) : base(message) { }
        public InvalidEventOutException(string message, Exception inner) : base(message, inner) { }
    }
}
