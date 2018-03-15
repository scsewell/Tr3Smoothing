using System;
using Gtk;
using SoSmooth.IO.tr3;
using SoSmooth.IO.Vrml;

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

            ImageMenuItem file_openTr3 = new ImageMenuItem("Open _Tr3");
            file_openTr3.Image = new Image(Stock.Open, IconSize.Menu);
            file_openTr3.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.O, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            file_openTr3.Activated += OnOpenTr3;
            file_menu.Append(file_openTr3);

            ImageMenuItem file_openVrml = new ImageMenuItem("Open _Meshes");
            file_openVrml.Image = new Image(Stock.Open, IconSize.Menu);
            file_openVrml.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.M, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            file_openVrml.Activated += OnOpenVRML;
            file_menu.Append(file_openVrml);

            file_menu.Append(new SeparatorMenuItem());

            ImageMenuItem file_quit = new ImageMenuItem("_Quit");
            file_quit.Image = new Image(Stock.Quit, IconSize.Menu);
            file_quit.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.Q, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            file_quit.Activated += OnQuit;
            file_menu.Append(file_quit);

            Append(file_item);
        }

        private void OnOpenTr3(object sender, EventArgs e)
        {
            Logger.Info("Selecting Tr3 file to open");

            FileChooserDialog fileChooser = new FileChooserDialog(
                "Open Tr3 File",
                m_mainWindow,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept
                );

            FileFilter filter = new FileFilter();
            filter.AddPattern("*.tr3");
            filter.Name = "Tr3";
            fileChooser.AddFilter(filter);

            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                Model model = Tr3FileHandler.Instance.Read(fileChooser.Filename);
            }

            fileChooser.Destroy();
        }

        private void OnOpenVRML(object sender, EventArgs e)
        {
            Logger.Info("Selecting VRML file to open");

            FileChooserDialog fileChooser = new FileChooserDialog(
                "Open Mesh File",
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
                MeshManager.Instance.AddMeshes(VrmlExtractor.Instance.Read(fileChooser.Filename));
            }

            fileChooser.Destroy();
        }

        private void OnQuit(object sender, EventArgs e)
        {
            m_mainWindow.Quit();
        }
    }
}
