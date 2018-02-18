using System;
using SoSmooth.IO.Vrml.Fields;
using SoSmooth.IO.Vrml.Nodes;
using SoSmooth.IO.Vrml.Parser.Statements;
using SoSmooth.IO.Vrml.Parser.Statements.Extern;
using SoSmooth.IO.Vrml.Parser.Statements.Proto;
using SoSmooth.IO.Vrml.Tokenizer;

namespace SoSmooth.IO.Vrml.Parser
{
    public class VrmlParser
    {
        private readonly VrmlTokenizer m_tokenizer;
        private FieldParser m_fieldParser;

        public VrmlParser(VrmlTokenizer tokenizer)
        {
            m_tokenizer = tokenizer;
        }

        public VrmlScene Parse()
        {
            VrmlScene scene = new VrmlScene();
            Parse(scene.Root.Children);
            return scene;
        }

        public void Parse(MFNode container)
        {
            var context = new ParserContext(m_tokenizer);
            try
            {
                m_fieldParser = new FieldParser(context, ParseNodeStatement);

                context.PushNodeContainer(container);

                ParseStatements(context);

                context.PopNodeContainer();

            }
            catch (VrmlParseException exc)
            {
                throw new InvalidVrmlSyntaxException(exc.Message + " at Line: " + context.LineIndex + " Column: " + context.ColumnIndex);
            }
        }

        protected virtual void ParseStatements(ParserContext context)
        {
            bool validToken;
            do
            {
                var token = context.PeekNextToken();
                switch (token.Text)
                {
                    case "DEF":
                    case "USE":
                    case "PROTO":
                    case "EXTERNPROTO":
                    case "ROUTE":
                        validToken = true;
                        ParseStatement(context);
                        break;
                    default:
                        if (token.Type == VrmlTokenType.Word)
                        {
                            ParseStatement(context);
                            validToken = true;
                        }
                        else
                        {
                            validToken = false;
                        }
                        break;
                }
            } while (validToken);
        }

        protected virtual void ParseStatement(ParserContext context)
        {
            var token = context.PeekNextToken();
            switch (token.Text)
            {
                case "DEF":
                case "USE":
                    ParseNodeStatement(context);
                    break;
                case "PROTO":
                case "EXTERNPROTO":
                    ParseProtoStatement(context);
                    break;
                case "ROUTE":
                    ParseRouteStatement(context);
                    break;
                default:
                    if (token.Type == VrmlTokenType.Word)
                    {
                        ParseNodeStatement(context);
                    }
                    else
                    {
                        throw new Exception("Unexpected context");
                    }
                    break;
            }
        }

        protected virtual void ParseDefNodeStatement(ParserContext context)
        {
            context.ReadNextToken();
            context.NodeName = ParseNodeNameId(context);
            ParseNode(context);
        }

        protected virtual void ParseNodeStatement(ParserContext context)
        {
            var token = context.PeekNextToken();
            switch (token.Text)
            {
                case "DEF":
                    ParseDefNodeStatement(context);
                    break;
                case "USE":
                    var useStatement = UseStatement.Parse(context);
                    var node = context.FindNode(useStatement.NodeName);
                    context.AcceptChild(node);
                    break;
                default:
                    ParseNode(context);
                    break;
            }
        }

        protected virtual void ParseRootNodeStatement(ParserContext context)
        {
            var token = context.PeekNextToken();
            switch (token.Text)
            {
                case "DEF":
                    ParseDefNodeStatement(context);
                    break;
                default:
                    ParseNode(context);
                    break;
            }
        }

        protected virtual void ParseProtoStatement(ParserContext context)
        {
            var keyword = context.PeekNextToken();
            switch (keyword.Text)
            {
                case "PROTO":
                    ParseProto(context);
                    break;
                case "EXTERNPROTO":
                    ParseExternProto(context);
                    break;
                default:
                    throw new InvalidVrmlSyntaxException("PROTO or EXTERNPROTO expected");
            }
        }

        protected virtual void ParseProtoStatements(ParserContext context)
        {
            var validToken = true;
            do
            {
                var token = context.PeekNextToken();
                switch (token.Text)
                {
                    case "PROTO":
                    case "EXTERNPROTO":
                        ParseProtoStatement(context);
                        break;
                    default:
                        validToken = false;
                        break;
                }
            } while (validToken);
        }

        protected virtual void ParseProto(ParserContext context)
        {
            context.ReadKeyword("PROTO");

            var proto = new ProtoNode
            {
                Name = ParseNodeNameId(context)
            };
            context.PushNodeContainer(proto.Children);
            context.PushFieldContainer(proto);
            ParseInterfaceDeclarations(context);
            if (context.ReadNextToken().Type != VrmlTokenType.OpenBrace)
            {
                throw new InvalidVrmlSyntaxException();
            }
            ParseProtoBody(context);
            if (context.ReadNextToken().Type != VrmlTokenType.CloseBrace)
            {
                throw new InvalidVrmlSyntaxException();
            }
            context.PopFieldContainer();
            context.PopNodeContainer();
            context.RegisterPtototype(proto);
        }

        protected virtual void ParseProtoBody(ParserContext context)
        {
            ParseProtoStatements(context);
            ParseRootNodeStatement(context);
            ParseStatements(context);
        }

        private void ParseInterfaceDeclarations(ParserContext context)
        {
            var statement = ProtoInterfaceDeclarationsStatement.Parse(context, ParseNodeStatement);

            var node = context.FieldContainer as ProtoNode;
            if (node == null) throw new Exception("Unexpected context");

            foreach (var expFld in statement.Fields)
            {
                var field = expFld.Value;
                node.AddField(expFld.FieldId, field);
            }

            foreach (var expFld in statement.ExposedFields)
            {
                var field = expFld.Value;
                node.AddExposedField(expFld.FieldId, field);
            }

        }

        private void ParseRestrictedInterfaceDeclaration(ParserContext context)
        {
            var node = context.FieldContainer as ProtoNode;
            if (node == null) throw new Exception("Unexpected context");
            var accessType = context.ReadNextToken().Text;
            switch (accessType)
            {
                case "eventIn":
                    var fieldInType = ParseFieldType(context);
                    var eventInId = ParseEventInId(context);
                    break;
                case "eventOut":
                    var fieldOutType = ParseFieldType(context);
                    var eventOutId = ParseEventOutId(context);
                    break;
                case "field":
                    var fieldType = ParseFieldType(context);
                    var fieldId = ParseFieldId(context);
                    var field = context.CreateField(fieldType);
                    node.AddField(fieldId, field);
                    field.AcceptVisitor(m_fieldParser);
                    break;
                default:
                    throw new Exception("Unexpected context");
            }
        }

        protected virtual void ParseExternProto(ParserContext context)
        {
            context.ReadKeyword("EXTERNPROTO");

            var nodeTypeId = ParseNodeTypeId(context);
            ParseExternInterfaceDeclarations(context);
            ParseURLList(context);
        }

        protected virtual void ParseExternInterfaceDeclarations(ParserContext context)
        {
            var statement = ExternInterfaceDeclarationsStatement.Parse(context);
        }

        protected virtual void ParseRouteStatement(ParserContext context)
        {
            var statement = RouteStatement.Parse(context);
        }

        protected virtual void ParseURLList(ParserContext context)
        {
            MFString urls = new MFString();
            urls.AcceptVisitor(m_fieldParser);
        }

        protected virtual void ParseScriptNode(ParserContext context)
        {
            var keyword = context.ReadNextToken();
            if (keyword.Text != "Script")
            {
                throw new InvalidVrmlSyntaxException("Script expected");
            }
            context.ReadOpenBrace();
            ParseScriptBody(context);
            context.ReadCloseBrace();
        }

        protected virtual void ParseScriptBody(ParserContext context)
        {
            var validToken = true;
            do
            {
                var token = context.PeekNextToken();
                if (token.Type == VrmlTokenType.Word)
                {
                    ParseScriptBodyElement(context);
                }
                else
                {
                    validToken = false;
                }
            } while (validToken);
        }

        protected virtual void ParseNode(ParserContext context)
        {
            var token = context.PeekNextToken();
            if (token.Type == VrmlTokenType.EOF) return;
            switch (token.Text)
            {
                case "Script":
                    ParseScriptNode(context);
                    break;
                default:
                    var nodeTypeId = ParseNodeTypeId(context);
                    var node = context.CreateNode(nodeTypeId, context.NodeName);
                    context.NodeName = null;
                    context.PushFieldContainer(node);
                    if (context.ReadNextToken().Type != VrmlTokenType.OpenBrace)
                    {
                        throw new InvalidVrmlSyntaxException("Open brace expected");
                    }
                    ParseNodeBody(context);
                    if (context.ReadNextToken().Type != VrmlTokenType.CloseBrace)
                    {
                        throw new InvalidVrmlSyntaxException();
                    }
                    context.PopFieldContainer();
                    context.AcceptChild(node);
                    break;
            }
        }

        protected virtual void ParseNodeBody(ParserContext context)
        {
            var validToken = true;
            do
            {
                var token = context.PeekNextToken();
                if (token.Type == VrmlTokenType.Word)
                {
                    ParseNodeBodyElement(context);
                }
                else
                {
                    validToken = false;
                }
            } while (validToken);
        }
        
        protected virtual void ParseScriptBodyElement(ParserContext context)
        {
            var token = context.PeekNextToken();
            VrmlToken tokenIs = null;
            string fieldType;
            switch (token.Text)
            {
                case "eventIn":
                    tokenIs = context.PeekNextToken(3);
                    if (tokenIs.Text == "IS")
                    {
                        token = context.ReadNextToken();
                        fieldType = ParseFieldType(context);
                        string eventInId1 = ParseEventInId(context);
                        tokenIs = context.ReadNextToken();
                        string eventInId2 = ParseEventInId(context);
                    }
                    else
                    {
                        ParseRestrictedInterfaceDeclaration(context);
                    }
                    break;
                case "eventOut":
                    tokenIs = context.PeekNextToken(3);
                    if (tokenIs.Text == "IS")
                    {
                        token = context.ReadNextToken();
                        fieldType = ParseFieldType(context);
                        string eventOutId1 = ParseEventOutId(context);
                        tokenIs = context.ReadNextToken();
                        string eventOutId2 = ParseEventOutId(context);
                    }
                    else
                    {
                        ParseRestrictedInterfaceDeclaration(context);
                    }
                    break;
                case "field":
                    tokenIs = context.PeekNextToken(3);
                    if (tokenIs.Text == "IS")
                    {
                        token = context.ReadNextToken();
                        fieldType = ParseFieldType(context);
                        string fieldId1 = ParseFieldId(context);
                        tokenIs = context.ReadNextToken();
                        string fieldId2 = ParseFieldId(context);
                    }
                    else
                    {
                        ParseRestrictedInterfaceDeclaration(context);
                    }
                    break;
                default:
                    if (token.Type == VrmlTokenType.Word)
                    {
                        ParseNodeBodyElement(context);
                    }
                    else
                    {
                        throw new Exception("Unexpected context");
                    }
                    break;
            }
        }

        protected virtual void ParseNodeBodyElement(ParserContext context)
        {
            var node = context.FieldContainer;
            if (node == null)
            {
                throw new Exception("Invalid context");
            }
            var token = context.PeekNextToken();
            switch (token.Text)
            {
                case "ROUTE":
                    ParseRouteStatement(context);
                    break;
                case "PROTO":
                    ParseProto(context);
                    break;
            }
            string fieldId = ParseFieldId(context);
            token = context.PeekNextToken();
            switch (token.Text)
            {
                case "IS":
                    token = context.ReadNextToken();
                    string interfaceFieldId = ParseFieldId(context);
                    break;
                default:
                    var field = node.GetExposedField(fieldId);
                    field.AcceptVisitor(m_fieldParser);
                    break;
            }
        }

        private static string ParseNodeNameId(ParserContext context)
        {
            return context.ParseNodeNameId();
        }

        private string ParseNodeTypeId(ParserContext context)
        {
            return ParseId(context);
        }

        private string ParseFieldId(ParserContext context)
        {
            return context.ParseFieldId();
        }

        protected virtual string ParseEventInId(ParserContext context)
        {
            return context.ParseEventInId();
        }

        private static string ParseEventOutId(ParserContext context)
        {
            return context.ParseEventOutId();
        }

        private static string ParseId(ParserContext context)
        {
            return context.ReadNextToken().Text;
        }

        private string ParseFieldType(ParserContext context)
        {
            return context.ParseFieldType();
        }
    }
}
