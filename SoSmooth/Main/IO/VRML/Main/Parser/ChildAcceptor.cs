using System;
using SoSmooth.Vrml.Fields;
using SoSmooth.Vrml.Nodes;

namespace SoSmooth.Vrml.Parser
{
    public class ChildAcceptor : IFieldVisitor
    {
        public Node Child { get; set; }

        public void Visit(SFBool field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFImage field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFFloat field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFFloat field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFString field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFString field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFInt32 field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFInt32 field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFVec2f field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFVec2f field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFVec3f field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFVec3f field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFColor field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFColor field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFNode field)
        {
            field.Node = Child;
        }

        public void Visit(MFNode field)
        {
            field.AppendValue(Child);
        }

        public void Visit(SFRotation field)
        {
            throw new NotImplementedException();
        }

        public void Visit(MFRotation field)
        {
            throw new NotImplementedException();
        }

        public void Visit(SFTime field)
        {
            throw new NotImplementedException();
        }
    }
}
