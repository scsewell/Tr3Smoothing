﻿using System;

namespace SoSmooth.IO.Vrml
{
    public class InvalidEventInException : VrmlParseException
    {
        public InvalidEventInException() { }
        public InvalidEventInException(string message) : base(message) { }
        public InvalidEventInException(string message, Exception inner) : base(message, inner) { }
    }
}
