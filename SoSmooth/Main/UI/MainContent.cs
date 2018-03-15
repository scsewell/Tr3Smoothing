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
            m_sceneWindow = SceneWindow.CreateSceneWindow();
            MeshList meshList = new MeshList();

            VBox leftControls = new VBox(false, 6);
            leftControls.WidthRequest = 200;
            leftControls.PackStart(meshList, true, true, 0);

            Frame frame = new Frame();
            frame.Shadow = ShadowType.EtchedIn;
            frame.Add(leftControls);
            
            Pack1(frame, false, false);
            Pack2(m_sceneWindow, true, true);

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
