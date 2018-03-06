using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SoSmooth.Rendering;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// A component that can be drawn in the scene.
    /// </summary>
    public abstract class Renderer : Component, IComparable
    {
        private readonly Surface m_surface;

        private readonly BlendSetting m_blend = new BlendSetting();
        private readonly DepthMaskSetting m_depthMask = new DepthMaskSetting();
        private readonly CullModeSetting m_cullMode = new CullModeSetting();
        private readonly PolygonModeSetting m_polygonMode = new PolygonModeSetting();

        private readonly Matrix4Uniform m_modelMatrix = new Matrix4Uniform("u_modelMatrix");
        private readonly ColorUniform m_color = new ColorUniform("u_color");

        private string m_programName = null;
        private ShaderProgram m_shaderProgram = null;

        /// <summary>
        /// If true this component is allowed to render.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The name of the desired shader program. Set to null if the shader name is invalid.
        /// </summary>
        public string ShaderProgram
        {
            get { return m_programName; }
            set
            {
                if (m_programName != value)
                {
                    if (ShaderManager.Instance.GetProgram(value, out m_shaderProgram))
                    {
                        m_programName = value;
                    }
                    else
                    {
                        m_programName = null;
                    }
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
        /// Sets the polygon mode of the rendered mesh's front faces. Default is <see cref="PolygonMode.Fill"/>.
        /// </summary>
        public PolygonMode FrontFaceMode
        {
            get { return m_polygonMode.FrontFaceMode; }
            set { m_polygonMode.FrontFaceMode = value; }
        }

        /// <summary>
        /// Sets the polygon mode of the rendered mesh's back faces. Default is <see cref="PolygonMode.Fill"/>.
        /// </summary>
        public PolygonMode BackFaceMode
        {
            get { return m_polygonMode.BackFaceMode; }
            set { m_polygonMode.BackFaceMode = value; }
        }

        /// <summary>
        /// The color of the rendered elements. Default is <see cref="Color4.White"/>.
        /// </summary>
        public Color4 Color
        {
            get { return m_color.Value; }
            set { m_color.Value = value; }
        }

        /// <summary>
        /// The surface that is rendered.
        /// </summary>
        protected Surface Surface => m_surface;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Renderer(Entity entity, Surface surface) : base(entity)
        {
            m_surface = surface;

            m_surface.AddSetting(m_cullMode);
            m_surface.AddSetting(m_polygonMode);
            m_surface.AddSetting(m_depthMask);
            m_surface.AddSetting(m_blend);

            m_surface.AddSetting(m_modelMatrix);
            m_surface.AddSetting(m_color);

            ShaderProgram = ShaderManager.SHADER_UNLIT;
        }

        /// <summary>
        /// Checks if this component is culled and should not render.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        /// <returns>True if this component is culled.</returns>
        public bool IsCulled(Camera camera)
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            if (!Enabled || m_shaderProgram == null)
            {
                return true;
            }

            return OnCull(camera);
        }

        /// <summary>
        /// Checks if this component is culled.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        protected virtual bool OnCull(Camera camera)
        {
            return false;
        }
        
        /// <summary>
        /// Renders this component.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        public void Render(Camera camera)
        {
            if (Disposed) { throw new ObjectDisposedException(GetType().FullName); }

            m_modelMatrix.Value = Transform.LocalToWorldMatix;
            m_surface.SetShaderProgram(m_shaderProgram);
            OnRender(camera);
            m_surface.Render();
        }

        /// <summary>
        /// Render this component.
        /// </summary>
        /// <param name="camera">The camera that is currently rendering.</param>
        protected virtual void OnRender(Camera camera) { }

        /// <summary>
        /// Sorts renderables as to ensure the least state changes.
        /// </summary>
        public int CompareTo(object obj)
        {
            Renderer other = obj as Renderer;
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
                else if (FrontFaceMode != other.FrontFaceMode)
                {
                    return (int)FrontFaceMode - (int)other.FrontFaceMode;
                }
                else if (BackFaceMode != other.BackFaceMode)
                {
                    return (int)BackFaceMode - (int)other.BackFaceMode;
                }
                else if (m_shaderProgram != other.m_shaderProgram)
                {
                    return m_shaderProgram.GetHashCode() - other.m_shaderProgram.GetHashCode();
                }
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// Disposes this component and frees held resources
        /// </summary>
        /// <param name="entity">True if managed resources should be cleaned up.</param>
        protected override void OnDispose(bool disposing)
        {
            m_surface.Dispose();

            base.OnDispose(disposing);
        }
    }
}
