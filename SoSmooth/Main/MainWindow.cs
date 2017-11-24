using System;
using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// Maintains the Main UI window of the application.
    /// </summary>
    public class MainWindow : Window
    {
        private const int MIN_WIDTH     = 800;
        private const int MIN_HEIGHT    = 600;

        private SceneWindow m_sceneWindow;

        /// <summary>
        /// Constructor that assembles the separate UI regions.
        /// </summary>
        public MainWindow() : base(WindowType.Toplevel)
        {
            Name = Program.NAME;
            Title = Program.NAME;
            SetSizeRequest(MIN_WIDTH, MIN_HEIGHT);
            WindowPosition = WindowPosition.CenterOnParent;
            DeleteEvent += OnDeleteEvent;
            
            MainMenuBar menuBar = new MainMenuBar(this);
            m_sceneWindow = new SceneWindow();
            
            VBox vBox = new VBox(false, 0);
            vBox.PackStart(menuBar.MenuBar, false, false, 0);
            vBox.PackStart(m_sceneWindow.Widget, true, true, 0);

            Add(vBox);
            ShowAll();
        }

        private void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Quit();
            a.RetVal = true;
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
    }
}
