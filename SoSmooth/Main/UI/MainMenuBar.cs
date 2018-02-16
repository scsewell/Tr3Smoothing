using System;
using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// Manages the main menu elements.
    /// </summary>
    public class MainMenuBar : MenuBar
    {
        private MainWindow m_mainWindow;
        
        public MainMenuBar(MainWindow mainWindow)
        {
            m_mainWindow = mainWindow;

            AccelGroup accelGroup = new AccelGroup();
            m_mainWindow.AddAccelGroup(accelGroup);

            Menu file_menu = new Menu();
            MenuItem file_item = new MenuItem("_File");
            file_item.Submenu = file_menu;

            ImageMenuItem file_open = new ImageMenuItem(Stock.Open, accelGroup);
            file_open.Activated += OnOpen;
            file_menu.Append(file_open);

            file_menu.Append(new SeparatorMenuItem());

            ImageMenuItem file_quit = new ImageMenuItem(Stock.Quit, accelGroup);
            file_quit.Activated += OnQuit;
            file_menu.Append(file_quit);

            Append(file_item);
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Logger.Info("Selecting VRML file to open");

            FileChooserDialog fileChooser = new FileChooserDialog(
                "Open file",
                m_mainWindow,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept
                );

            FileFilter filter = new FileFilter();
            filter.AddPattern("*.wrl");
            filter.Name = "VRML";
            fileChooser.AddFilter(filter);

            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                SmoothingManager.Instance.m_meshes.AddRange(Vrml.VrmlExtractor.Instance.Read(fileChooser.Filename));
            }

            fileChooser.Destroy();
        }

        private void OnQuit(object sender, EventArgs e)
        {
            m_mainWindow.Quit();
        }
    }
}
