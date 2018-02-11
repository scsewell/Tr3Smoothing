using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Collections.ObjectModel;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Base class for all surfaces. A Surface is an abstract object that can render 
    /// itself to the screen using a shader program and can have a number of settings 
    /// to modify its behaviour.
    /// </summary>
    public abstract class Surface
    {
        /// <summary>
        /// The shader program used to draw this surface.
        /// </summary>
        protected ShaderProgram Program { get; private set; }

        private readonly List<SurfaceSetting> m_settingsSet = new List<SurfaceSetting>();
        private readonly List<SurfaceSetting> m_settingsUnSet = new List<SurfaceSetting>();

        public ReadOnlyCollection<SurfaceSetting> Settings { get { return m_settingsSet.AsReadOnly(); } }

        /// <summary>
        /// Sets the shader program used to render this surface.
        /// </summary>
        /// <param name="program">The program.</param>
        public void SetShaderProgram(ShaderProgram program)
        {
            if (Program != program)
            {
                Program = program;
                OnNewShaderProgram();
            }
        }

        /// <summary>
        /// Is called after a shader program has been set. Use this for sub class specific behaviour.
        /// </summary>
        protected virtual void OnNewShaderProgram() { }

        /// <summary>
        /// Adds <see cref="SurfaceSetting"/>s to this surface.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void AddSettings(IEnumerable<SurfaceSetting> settings)
        {
            foreach (SurfaceSetting setting in settings)
            {
                AddSetting(setting);
            }
        }

        /// <summary>
        /// Adds <see cref="SurfaceSetting"/>s to this surface.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void AddSettings(params SurfaceSetting[] settings)
        {
            foreach (SurfaceSetting setting in settings)
            {
                AddSetting(setting);
            }
        }

        /// <summary>
        /// Adds a <see cref="SurfaceSetting"/> to this surface.
        /// </summary>
        /// <param name="setting">The setting.</param>
        public void AddSetting(SurfaceSetting setting)
        {
            m_settingsSet.Add(setting);
            if (setting.NeedsUnsetting)
            {
                m_settingsUnSet.Add(setting);
            }
        }

        /// <summary>
        /// Removes a <see cref="SurfaceSetting"/> from this surface.
        /// </summary>
        /// <param name="setting">The setting.</param>
        public void RemoveSetting(SurfaceSetting setting)
        {
            if (m_settingsSet.Remove(setting) && setting.NeedsUnsetting)
            {
                m_settingsUnSet.Remove(setting);
            }
        }

        /// <summary>
        /// Removes all <see cref="SurfaceSetting"/>s from this surface.
        /// </summary>
        public void ClearSettings()
        {
            m_settingsSet.Clear();
            m_settingsUnSet.Clear();
        }

        /// <summary>
        /// Renders this surface.
        /// It does so by activating its shader program, setting all its settings, 
        /// invoking sub class specific render behaviour and then unsetting its settings again.
        /// </summary>
        public void Render()
        {
            GL.UseProgram(Program);

            foreach (SurfaceSetting setting in m_settingsSet)
            {
                setting.Set(Program);
            }
            
            OnRender();

            foreach (SurfaceSetting setting in m_settingsUnSet)
            {
                setting.UnSet(Program);
            }
        }

        /// <summary>
        /// Is called after the surface's shader program and its settings have been set.
        /// Implement this to add sub class specific render behaviour.
        /// </summary>
        protected abstract void OnRender();
    }
}
