using System.Timers;
using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// The widget that contains all content below the menu bar.
    /// </summary>
    public class MainContent : HBox
    {
        /// <summary>
        /// The max frequency of the scene update/rendering in hertz.
        /// </summary>
        private const float MAX_UPDATE_FREQUENCY = 80.0f;

        private static SceneWindow m_sceneWindow;
        public static SceneWindow SceneWindow => m_sceneWindow;

        private Timer m_updateTimer = new Timer(1000 / MAX_UPDATE_FREQUENCY);
        private bool m_canUpdate = true;
        
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

            m_updateTimer.Elapsed += TimerUpdate;
            m_updateTimer.Start();
        }

        /// <summary>
        /// Triggered every time the application should update the scene UI
        /// </summary>
        private void TimerUpdate(object sender, ElapsedEventArgs e)
        {
            m_canUpdate = true;
        }

        /// <summary>
        /// Excecutes the main update loop for scene windows.
        /// </summary>
        private bool Update()
        {
            if (m_canUpdate)
            {
                Time.FrameStart();
                m_sceneWindow.QueueDraw();
                m_canUpdate = false;
            }
            return true;
        }
    }
}
