using System;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// A component that can be drawn in the scene.
    /// </summary>
    public abstract class Renderable : Component, IComparable
    {
        protected BlendSetting m_blend = new BlendSetting();
        protected DepthMaskSetting m_depthMask = new DepthMaskSetting();
        protected CullModeSetting m_cullMode = new CullModeSetting();
        protected PolygonModeSetting m_polygonMode = new PolygonModeSetting();

        private string m_programName = null;
        private ShaderProgram m_shaderProgram = null;

        /// <summary>
        /// The name of the desired shader program.
        /// </summary>
        public string ShaderProgram
        {
            get { return m_programName; }
            set
            {
                if (m_programName != value)
                {
                    m_programName = value;
                    m_shaderProgram = null;
                }
            }
        }

        /// <summary>
        /// The blend mode used to combine the fragment shader's output with the render target.
        /// Default is <see cref="BlendMode.None"/>.
        /// </summary>
        public BlendMode BlendMode
        {
            get { return m_blend.BlendMode; }
            set { m_blend.BlendMode = value; }
        }

        /// <summary>
        /// If true write to the depth buffer. Default is <see true/>. 
        /// </summary>
        public bool WriteDepth
        {
            get { return m_depthMask.WriteDepth; }
            set { m_depthMask.WriteDepth = value; }
        }

        /// <summary>
        /// Sets the culling mode of the rendered mesh. Default is <see cref="CullMode.Back"/>.
        /// </summary>
        public CullMode CullMode
        {
            get { return m_cullMode.CullMode; }
            set { m_cullMode.CullMode = value; }
        }

        /// <summary>
        /// Sets the face mode of the rendered mesh. Default is <see cref="PolygonMode.Fill"/>.
        /// </summary>
        public PolygonMode PolygonMode
        {
            get { return m_polygonMode.FrontFaceMode; }
            set { m_polygonMode.FrontFaceMode = value; }
        }

        /// <summary>
        /// The surface that is rendered.
        /// </summary>
        protected abstract Surface Surface { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Renderable(Entity entity) : base(entity)
        {
        }

        /// <summary>
        /// Renders this component.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        public void Render(Camera camera)
        {
            if (m_programName != null)
            {
                if (m_shaderProgram == null)
                {
                    m_shaderProgram = ShaderManager.Instance.GetProgram(m_programName);
                }

                if (m_shaderProgram != null)
                {
                    Surface.SetShaderProgram(m_shaderProgram);

                    OnRender(camera);
                }
            }
        }

        /// <summary>
        /// Render this component.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        protected abstract void OnRender(Camera camera);

        /// <summary>
        /// Sorts renderables as to ensure the least state changes.
        /// </summary>
        public int CompareTo(object obj)
        {
            Renderable other = obj as Renderable;
            if (other != null)
            {
                if (BlendMode != other.BlendMode)
                {
                    return (int)BlendMode - (int)other.BlendMode;
                }
                else if (WriteDepth != other.WriteDepth)
                {
                    return (WriteDepth ? 1 : 0) - (other.WriteDepth ? 1 : 0);
                }
                else if (CullMode != other.CullMode)
                {
                    return (int)CullMode - (int)other.CullMode;
                }
                else if (PolygonMode != other.PolygonMode)
                {
                    return (int)PolygonMode - (int)other.PolygonMode;
                }
                else if (m_shaderProgram != other.m_shaderProgram && m_shaderProgram != null && other.m_shaderProgram != null)
                {
                    return m_shaderProgram.GetHashCode() - other.m_shaderProgram.GetHashCode();
                }
                return 0;
            }
            return -1;
        }
    }
}
