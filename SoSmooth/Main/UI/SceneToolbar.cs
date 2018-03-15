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
        public SceneToolbar(MainWindow mainWindow, SceneWindow sceneWindow)
        {
            m_sceneWindow = sceneWindow;

            HeightRequest = 30;

            MenuBar menuBar = new MenuBar();
            AccelGroup accelGroup = new AccelGroup();
            mainWindow.AddAccelGroup(accelGroup);

            // view menu
            MenuItem view_item = new MenuItem("_View");
            menuBar.Append(view_item);
            Menu view_menu = new Menu();
            view_item.Submenu = view_menu;

            MenuItem view_focus = new MenuItem("_Focus Selection");
            view_focus.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.F, Gdk.ModifierType.None, AccelFlags.Visible));
            view_focus.Activated += ((o, e) => sceneWindow.Camera.EaseToMeshes(MeshManager.Instance.Selected));
            view_menu.Append(view_focus);

            view_menu.Append(new SeparatorMenuItem());

            MenuItem view_front = new MenuItem("_Front");
            view_front.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.KP_1, Gdk.ModifierType.None, AccelFlags.Visible));
            view_front.Activated += ((o, e) => sceneWindow.Camera.EaseCamera(SceneCamera.FRONT_VIEW));
            view_menu.Append(view_front);

            MenuItem view_back = new MenuItem("_Back");
            view_back.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.KP_1, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            view_back.Activated += ((o, e) => sceneWindow.Camera.EaseCamera(SceneCamera.BACK_VIEW));
            view_menu.Append(view_back);

            MenuItem view_right = new MenuItem("_Right");
            view_right.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.KP_3, Gdk.ModifierType.None, AccelFlags.Visible));
            view_right.Activated += ((o, e) => sceneWindow.Camera.EaseCamera(SceneCamera.RIGHT_VIEW));
            view_menu.Append(view_right);

            MenuItem view_left = new MenuItem("_Left");
            view_left.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.KP_3, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            view_left.Activated += ((o, e) => sceneWindow.Camera.EaseCamera(SceneCamera.LEFT_VIEW));
            view_menu.Append(view_left);

            MenuItem view_top = new MenuItem("_Top");
            view_top.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.KP_7, Gdk.ModifierType.None, AccelFlags.Visible));
            view_top.Activated += ((o, e) => sceneWindow.Camera.EaseCamera(SceneCamera.TOP_VIEW));
            view_menu.Append(view_top);

            MenuItem view_bottom = new MenuItem("B_ottom");
            view_bottom.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.KP_7, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            view_bottom.Activated += ((o, e) => sceneWindow.Camera.EaseCamera(SceneCamera.BOTTOM_VIEW));
            view_menu.Append(view_bottom);

            // render settings
            Label frontShadingLabel = new Label("Front Face Mode:");
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

            // pack elements
            HBox hBox = new HBox();
            hBox.PackStart(menuBar, false, false, 2);
            hBox.PackStart(new VSeparator(), false, false, 6);
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
