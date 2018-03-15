using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// Maintains the Main UI window of the application.
    /// </summary>
    public class MainWindow : Window
    {
        /// <summary>
        /// The minimal pixel width of the window permitted.
        /// </summary>
        private const int MIN_WIDTH     = 600;
        /// <summary>
        /// The minimal pixel height of the window permitted.
        /// </summary>
        private const int MIN_HEIGHT    = 400;

        /// <summary>
        /// The default pixel width of the window.
        /// </summary>
        private const int DEFAULT_WIDTH = 800;
        /// <summary>
        /// The default pixel height of the window.
        /// </summary>
        private const int DEFAULT_HEIGHT = 600;

        /// <summary>
        /// Constructor that assembles the separate UI regions.
        /// </summary>
        public MainWindow() : base(WindowType.Toplevel)
        {
            Name = Program.NAME;
            Title = Program.NAME;
            SetSizeRequest(MIN_WIDTH, MIN_HEIGHT);
            SetDefaultSize(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            WindowPosition = WindowPosition.CenterOnParent;
            
            DeleteEvent += OnDeleteEvent;
            
            MainMenuBar menuBar = new MainMenuBar(this);
            MainContent content = new MainContent();
            
            VBox vBox = new VBox(false, 0);
            vBox.PackStart(menuBar, false, false, 0);
            vBox.PackStart(content, true, true, 0);

            Add(vBox);
            ShowAll();
        }
        
        /// <summary>
        /// Called when the window exit button was pressed
        /// </summary>
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
            Logger.Info("Quitting application...");
            Application.Quit();
        }
    }
}
