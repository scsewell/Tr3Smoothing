using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// The widget that contains all content below the menu bar.
    /// </summary>
    public class MainContent : HBox
    {
        private static SceneWindow m_sceneWindow;
        public static SceneWindow SceneWindow => m_sceneWindow;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainContent(MainWindow window)
        {
            // the left controls
            ActiveMeshControls activeMesh = new ActiveMeshControls();
            HSeparator separator = new HSeparator();
            MeshList meshList = new MeshList();

            VBox leftControls = new VBox(false, 0);
            leftControls.WidthRequest = 250;
            leftControls.PackStart(activeMesh, false, false, 0);
            leftControls.PackStart(separator, false, false, 0);
            leftControls.PackStart(meshList, true, true, 0);

            Frame leftFrame = new Frame();
            leftFrame.Shadow = ShadowType.EtchedIn;
            leftFrame.Add(leftControls);

            // the scene content
            VBox sceneBox = new VBox();
            m_sceneWindow = SceneWindow.CreateSceneWindow();
            SceneToolbar sceneToolbar = new SceneToolbar(window, m_sceneWindow);

            sceneBox.PackStart(sceneToolbar, false, false, 0);
            sceneBox.PackStart(m_sceneWindow, true, true, 0);

            // the right controls
            SmoothingContent smoothing = new SmoothingContent();

            VBox rightControls = new VBox(false, 0);
            rightControls.WidthRequest = 250;
            rightControls.PackStart(smoothing, true, true, 0);

            Frame rightFrame = new Frame();
            rightFrame.Shadow = ShadowType.EtchedIn;
            rightFrame.Add(rightControls);

            // fit everything into the main window
            HPaned pan1 = new HPaned();
            HPaned pan2 = new HPaned();

            pan1.Pack1(leftFrame, false, false);
            pan1.Pack2(sceneBox, true, true);

            pan2.Pack1(pan1, true, true);
            pan2.Pack2(rightFrame, false, false);
            
            PackStart(pan2, true, true, 0);

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
