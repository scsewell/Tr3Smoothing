using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using SoSmooth.Vrml.Fields;
using SoSmooth.Vrml.Nodes;
using SoSmooth.Vrml.Tokenizer;

namespace SoSmooth.Vrml.Parser
{
    public class ParserContext
    {
        private readonly NodeFactory m_nodeFactory = new NodeFactory();
        private readonly ChildAcceptor m_childAcceptor = new ChildAcceptor();

        private readonly Queue<VrmlToken> m_queue = new Queue<VrmlToken>();
        private readonly Stack<Node> fieldContainers = new Stack<Node>();
        private readonly Stack<Field> nodeContainers = new Stack<Field>();
        private readonly Dictionary<string, Node> namedNodes = new Dictionary<string, Node>();

        private readonly VrmlTokenizer m_tokenizer;
        public int LineIndex => m_tokenizer.LineIndex;
        public int ColumnIndex => m_tokenizer.ColumnIndex;

        public string NodeName { get; set; }
        
        public Node FieldContainer
        {
            get
            {
                if (fieldContainers.Count > 0) { return fieldContainers.Peek(); }
                return null;
            }
        }

        public Field NodeContainer
        {
            get
            {
                if (nodeContainers.Count > 0) { return nodeContainers.Peek(); }
                return null;
            }
        }

        public ParserContext(VrmlTokenizer tokenizer)
        {
            m_tokenizer = tokenizer;
        }

        public void ReadKeyword(string keyword)
        {
            VrmlToken token = ReadNextToken();
            if (token.Type != VrmlTokenType.Word || token.Text != keyword)
            {
                throw new InvalidVrmlSyntaxException(keyword + " expected");
            }
        }

        public void ReadOpenBracket()
        {
            if (ReadNextToken().Type != VrmlTokenType.OpenBracket)
            {
                throw new InvalidVrmlSyntaxException();
            }
        }

        public void ReadCloseBracket()
        {
            if (ReadNextToken().Type != VrmlTokenType.CloseBracket)
            {
                throw new InvalidVrmlSyntaxException();
            }
        }
        
        public void ReadOpenBrace()
        {
            if (ReadNextToken().Type != VrmlTokenType.OpenBrace)
            {
                throw new InvalidVrmlSyntaxException();
            }
        }

        public void ReadCloseBrace()
        {
            if (ReadNextToken().Type != VrmlTokenType.CloseBrace)
            {
                throw new InvalidVrmlSyntaxException();
            }
        }
        
        public string ParseEventInId()
        {
            return ParseId();
        }

        public virtual string ParseEventOutId()
        {
            return ParseId();
        }

        public string ParseNodeNameId()
        {
            return ParseId();
        }

        public string ParseFieldType()
        {
            return ReadNextToken().Text;
        }

        public string ParseFieldId()
        {
            return ParseId();
        }
        
        protected string ParseId()
        {
            return ReadNextToken().Text;
        }
        
        public VrmlToken ReadNextToken()
        {
            VrmlToken token = null;
            if (m_queue.Count > 0)
            {
                token = m_queue.Dequeue();
            }
            else
            {
                token = m_tokenizer.ReadNextToken();
            }
            return token;
        }
        
        public virtual VrmlToken PeekNextToken()
        {
            if (m_queue.Count == 0)
            {
                m_queue.Enqueue(m_tokenizer.ReadNextToken());
            }
            return m_queue.Peek();
        }
        
        public VrmlToken PeekNextToken(int index)
        {
            while (m_queue.Count < (index + 1))
            {
                m_queue.Enqueue(m_tokenizer.ReadNextToken());
            }
            foreach (var token in m_queue)
            {
                if (index == 0) return token;
                index--;
            }
            return null;
        }

        public float ReadFloat()
        {
            string value = ReadNextToken().Text;
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public double ReadDouble()
        {
            string value = ReadNextToken().Text;
            return double.Parse(value, CultureInfo.InvariantCulture);
        }

        public virtual int ReadInt32()
        {
            string value = ReadNextToken().Text;
            return int.Parse(value);
        }

        public virtual string ReadString()
        {
            return ReadNextToken().Text;
        }

        public uint ReadHexadecimal()
        {
            string text = ReadNextToken().Text;
            if (text.StartsWith("0x"))
            {
                return uint.Parse(text.Substring(2), NumberStyles.HexNumber);
            }
            else
            {
                return uint.Parse(text);
            }
        }

        public void PushFieldContainer(Node fieldContainer)
        {
            fieldContainers.Push(fieldContainer);
        }
        
        public void PopFieldContainer()
        {
            fieldContainers.Pop();
        }
        
        public void PushNodeContainer(Field nodeContainer)
        {
            nodeContainers.Push(nodeContainer);
        }
        
        public void PopNodeContainer()
        {
            nodeContainers.Pop();
        }

        public Node CreateNode(string nodeTypeId, string nodeNameId)
        {
            var node = m_nodeFactory.CreateNode(nodeTypeId, nodeNameId);
            if (!string.IsNullOrEmpty(node.Name))
            {
                namedNodes[node.Name] = node;
            }
            return node;
        }

        public Field CreateField(string fieldType)
        {
            return Field.CreateField(fieldType);
        }
        
        public Node FindNode(string nodeNameId)
        {
            if (namedNodes.ContainsKey(nodeNameId))
            {
                return namedNodes[nodeNameId];
            }
            else
            {
                return null;
            }
        }

        public void AcceptChild(Node node)
        {
            m_childAcceptor.Child = node;
            NodeContainer.AcceptVisitor(m_childAcceptor);
        }

        public void RegisterPtototype(ProtoNode proto)
        {
            m_nodeFactory.AddPrototype(proto);
        }
    }
}
