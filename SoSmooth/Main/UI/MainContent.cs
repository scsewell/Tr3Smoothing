using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// The widget that contains all content below the menu bar.
    /// </summary>
    public class MainContent : HPaned
    {
        private SceneWindow m_sceneWindow;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainContent()
        {
            // the left controls
            ActiveMeshControls activeMesh = new ActiveMeshControls();
            HSeparator separator = new HSeparator();
            MeshList meshList = new MeshList();

            VBox leftControls = new VBox(false, 0);
            leftControls.WidthRequest = 300;
            leftControls.PackStart(activeMesh, false, false, 0);
            leftControls.PackStart(separator, false, false, 0);
            leftControls.PackStart(meshList, true, true, 0);

            Frame frame = new Frame();
            frame.Shadow = ShadowType.EtchedIn;
            frame.Add(leftControls);

            // the scene content
            VBox sceneBox = new VBox();
            m_sceneWindow = SceneWindow.CreateSceneWindow();
            SceneToolbar sceneToolbar = new SceneToolbar(m_sceneWindow);

            sceneBox.PackStart(sceneToolbar, false, false, 0);
            sceneBox.PackStart(m_sceneWindow, true, true, 0);

            // fit everything into the main window
            Pack1(frame, false, false);
            Pack2(sceneBox, true, true);

            KeyPressEvent += meshList.OnKeyPress;
            KeyPressEvent += m_sceneWindow.OnKeyPress;

            Time.SetStartTime();
            GLib.Idle.Add(Update);
        }

        /// <summary>
        /// Excecutes the main update loop for scene windows.
        /// </summary>
        private bool Update()
        {
            Time.FrameStart();
            m_sceneWindow.QueueDraw();
            return true;
        }
    }
}
