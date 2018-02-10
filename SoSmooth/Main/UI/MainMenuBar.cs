using System;
using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// Manages the menu elements at the top of the window.
    /// </summary>
    public class MainMenuBar
    {
        private MainWindow m_mainWindow;

        private MenuBar m_menuBar;
        public MenuBar MenuBar { get { return m_menuBar; } }

        public MainMenuBar(MainWindow mainWindow)
        {
            m_mainWindow = mainWindow;

            MenuItem file_quit = new MenuItem("Quit");
            file_quit.Activated += OnQuit;

            Menu file_menu = new Menu();
            file_menu.Append(file_quit);

            MenuItem file_item = new MenuItem("File");
            file_item.Submenu = file_menu;

            m_menuBar = new MenuBar();
            m_menuBar.Append(file_item);
        }

        private void OnQuit(object sender, EventArgs e)
        {
            m_mainWindow.Quit();
        }
    }
}
