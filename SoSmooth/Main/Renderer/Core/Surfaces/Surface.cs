using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace SoSmooth.Rendering
{
    /// <summary>
    /// Base class for all surfaces. A Surface is an abstract object that can render 
    /// itself to the screen using a shader program and can have a number of settings 
    /// to modify the drawing behaviour.
    /// </summary>
    public abstract class Surface : Disposable
    {
        /// <summary>
        /// The program currently set in the context.
        /// </summary>
        private static ShaderProgram m_currentProgram = null;

        private readonly List<SurfaceSetting> m_settings = new List<SurfaceSetting>();

        /// <summary>
        /// The shader program used to draw this surface.
        /// </summary>
        protected ShaderProgram Program { get; private set; }
        
        /// <summary>
        /// Sets the shader program used to render this surface.
        /// </summary>
        /// <param name="program">The program.</param>
        public void SetShaderProgram(ShaderProgram program)
        {
            ValidateDispose();

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
            ValidateDispose();

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
            ValidateDispose();

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
            ValidateDispose();

            m_settings.Add(setting);
        }

        /// <summary>
        /// Removes a <see cref="SurfaceSetting"/> from this surface.
        /// </summary>
        /// <param name="setting">The setting.</param>
        public void RemoveSetting(SurfaceSetting setting)
        {
            ValidateDispose();

            m_settings.Remove(setting);
        }

        /// <summary>
        /// Removes all <see cref="SurfaceSetting"/>s from this surface.
        /// </summary>
        public void ClearSettings()
        {
            ValidateDispose();

            m_settings.Clear();
        }

        /// <summary>
        /// Renders this surface.
        /// It does so by activating its shader program, setting all its settings, 
        /// and then invoking sub class specific render behaviour.
        /// </summary>
        public void Render()
        {
            ValidateDispose();

            // only set the program if it is different from what was last used
            if (m_currentProgram != Program)
            {
                m_currentProgram = Program;
                GL.UseProgram(m_currentProgram);
            }

            foreach (SurfaceSetting setting in m_settings)
            {
                setting.Set(Program);
            }

            OnRender();
        }

        /// <summary>
        /// Is called after the surface's shader program and its settings have been set.
        /// Implement this to add sub class specific render behaviour.
        /// </summary>
        protected abstract void OnRender();
    }
}
