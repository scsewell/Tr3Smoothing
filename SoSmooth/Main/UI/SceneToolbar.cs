using System;
using System.Collections.Generic;
using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// A toolbar that has options and tools for managing the scene.
    /// </summary>
    public class SceneToolbar : Toolbar
    {
        public static readonly string[] FRONT_SHADING_VALS = new string[] { "Solid", "Wireframe", "Hidden" };
        public static readonly string[] BACK_SHADING_VALS = new string[] { "Solid", "Wireframe" };

        private SceneWindow m_sceneWindow;
        private ComboBox m_frontShading;
        private ComboBox m_backShading;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SceneToolbar(SceneWindow window)
        {
            m_sceneWindow = window;

            HeightRequest = 30;

            Label frontShadingLabel = new Label("Front Faces Mode:");
            m_frontShading = new ComboBox(FRONT_SHADING_VALS);
            m_frontShading.TooltipText = "The render style for front faces.";
            m_frontShading.Active = 0;
            m_frontShading.CanFocus = false;
            m_frontShading.Changed += OnFrontShadingChanged;
            
            Label backShadingLabel = new Label("Back Face Mode:");
            m_backShading = new ComboBox(BACK_SHADING_VALS);
            m_backShading.TooltipText = "The render style for back faces.";
            m_backShading.Active = 0;
            m_backShading.CanFocus = false;
            m_backShading.Changed += OnBackShadingChanged;

            HBox hBox = new HBox();
            hBox.PackStart(frontShadingLabel, false, false, 2);
            hBox.PackStart(m_frontShading, false, false, 2);
            hBox.PackStart(new VSeparator(), false, false, 6);
            hBox.PackStart(backShadingLabel, false, false, 2);
            hBox.PackStart(m_backShading, false, false, 2);
            Add(hBox);
        }

        /// <summary>
        /// Called when the front shading value has been changed.
        /// </summary>
        private void OnFrontShadingChanged(object sender, EventArgs e)
        {
            TreeIter iter;
            m_frontShading.GetActiveIter(out iter);
            TreePath path = m_frontShading.Model.GetPath(iter);
            m_sceneWindow.Meshes.frontFaceMode = FRONT_SHADING_VALS[path.Indices[0]];
        }

        /// <summary>
        /// Called when the back shading value has been changed.
        /// </summary>
        private void OnBackShadingChanged(object sender, EventArgs e)
        {
            TreeIter iter;
            m_backShading.GetActiveIter(out iter);
            TreePath path = m_backShading.Model.GetPath(iter);
            m_sceneWindow.Meshes.backFaceMode = BACK_SHADING_VALS[path.Indices[0]];
        }
    }
}
