﻿using SoSmooth.IO.Vrml.Fields;

namespace SoSmooth.IO.Vrml.Nodes
{
    public abstract class TextureNode : Node
    {
        public SFBool RepeatS => GetField("repeatS") as SFBool;
        public SFBool RepeatT => GetField("repeatT") as SFBool;

        public TextureNode()
        {
            AddField("repeatS", new SFBool(true));
            AddField("repeatT", new SFBool(true));
        }
    }
}
