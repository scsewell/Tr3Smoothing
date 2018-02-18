using System.Collections.Generic;
using SoSmooth.IO.Vrml.Nodes;

namespace SoSmooth.IO.Vrml.Parser
{
    public class NodeFactory
    {
        private readonly Dictionary<string, Node> m_builtin = new Dictionary<string, Node>();
        private readonly Dictionary<string, Node> m_userDefined = new Dictionary<string, Node>();

        public NodeFactory()
        {
            m_builtin["Anchor"]                 = new AnchorNode();
            m_builtin["Appearance"]             = new AppearanceNode();
            m_builtin["Background"]             = new BackgroundNode();
            m_builtin["Billboard"]              = new BillboardNode();
            m_builtin["Box"]                    = new BoxNode();
            m_builtin["Color"]                  = new ColorNode();
            m_builtin["ColorInterpolator"]      = new ColorInterpolatorNode();
            m_builtin["Cone"]                   = new ConeNode();
            m_builtin["Coordinate"]             = new CoordinateNode();
            m_builtin["CoordinateInterpolator"] = new CoordinateInterpolatorNode();
            m_builtin["Cylinder"]               = new CylinderNode();
            m_builtin["DirectionalLight"]       = new DirectionalLightNode();
            m_builtin["Extrusion"]              = new ExtrusionNode();
            m_builtin["FontStyle"]              = new FontStyleNode();
            m_builtin["Group"]                  = new GroupNode();
            m_builtin["IndexedFaceSet"]         = new IndexedFaceSetNode();
            m_builtin["IndexedLineSet"]         = new IndexedLineSetNode();
            m_builtin["Material"]               = new MaterialNode();
            m_builtin["NavigationInfo"]         = new NavigationInfoNode();
            m_builtin["OrientationInterpolator"] = new OrientationInterpolatorNode();
            m_builtin["Normal"]                 = new NormalNode();
            m_builtin["PixelTexture"]           = new PixelTextureNode();
            m_builtin["PointLight"]             = new PointLightNode();
            m_builtin["PositionInterpolator"]   = new PositionInterpolatorNode();
            m_builtin["ScalarInterpolator"]     = new ScalarInterpolationNode();
            m_builtin["Shape"]                  = new ShapeNode();
            m_builtin["Sphere"]                 = new SphereNode();
            m_builtin["SpotLight"]              = new SpotLightNode();
            m_builtin["Text"]                   = new TextNode();
            m_builtin["TextureCoordinate"]      = new TextureCoordinateNode();
            m_builtin["TimeSensor"]             = new TimeSensorNode();
            m_builtin["TouchSensor"]            = new TouchSensorNode();
            m_builtin["Transform"]              = new TransformNode();
            m_builtin["Viewpoint"]              = new ViewpointNode();
            m_builtin["WorldInfo"]              = new WorldInfoNode();
        }

        public virtual Node CreateNode(string nodeTypeName, string nodeName)
        {
            if (m_builtin.ContainsKey(nodeTypeName))
            {
                Node node = m_builtin[nodeTypeName].Clone();
                node.Name = nodeName;
                return node;
            }
            if (m_userDefined.ContainsKey(nodeTypeName))
            {
                Node node = m_userDefined[nodeTypeName].Clone();
                node.Name = nodeName;
                return node;
            }
            throw new InvalidVrmlSyntaxException("Couldn't create node: " + nodeTypeName);
        }

        public void AddPrototype(Node proto)
        {
            m_userDefined[proto.Name] = proto;
        }
    }
}
