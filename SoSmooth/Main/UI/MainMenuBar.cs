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
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainMenuBar(MainWindow mainWindow)
        {
            m_mainWindow = mainWindow;

            AccelGroup accelGroup = new AccelGroup();
            m_mainWindow.AddAccelGroup(accelGroup);

            // file menu
            MenuItem file_item = new MenuItem("_File");
            Append(file_item);
            Menu file_menu = new Menu();
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

            // mesh menu
            MenuItem mesh_item = new MenuItem("_Mesh");
            Append(mesh_item);
            Menu mesh_menu = new Menu();
            mesh_item.Submenu = mesh_menu;

            MenuItem mesh_selectAll = new MenuItem("_Select All/None");
            mesh_selectAll.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.A, Gdk.ModifierType.None, AccelFlags.Visible));
            mesh_selectAll.Activated += ((o, e) => MeshManager.Instance.ToggleSelected());
            mesh_menu.Append(mesh_selectAll);

            MenuItem mesh_hide = new MenuItem("_Hide Selected");
            mesh_hide.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.h, Gdk.ModifierType.None, AccelFlags.Visible));
            mesh_hide.Activated += ((o, e) => MeshManager.Instance.HideSelected());
            mesh_menu.Append(mesh_hide);

            MenuItem mesh_unhide = new MenuItem("_Unhide All");
            mesh_unhide.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.H, Gdk.ModifierType.ShiftMask, AccelFlags.Visible));
            mesh_unhide.Activated += ((o, e) => MeshManager.Instance.ShowAll());
            mesh_menu.Append(mesh_unhide);

            MenuItem mesh_delete = new MenuItem("_Remove Selected");
            mesh_delete.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.Delete, Gdk.ModifierType.None, AccelFlags.Visible));
            mesh_delete.Activated += ((o, e) => MeshManager.Instance.DeleteSelected());
            mesh_menu.Append(mesh_delete);
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
