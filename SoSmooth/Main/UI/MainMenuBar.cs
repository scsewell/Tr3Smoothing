using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using SoSmooth.IO.tr3;
using SoSmooth.IO.Vrml;
using SoSmooth.Meshes;

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
            Menu file_menu = new Menu();
            file_item.Submenu = file_menu;
            Append(file_item);

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

            // edit menu
            MenuItem edit_item = new MenuItem("_Edit");
            Menu edit_menu = new Menu();
            edit_item.Submenu = edit_menu;
            Append(edit_item);

            MenuItem edit_undo = new MenuItem("_Undo");
            edit_undo.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.Z, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            edit_undo.Activated += ((o, e) => UndoStack.Instance.Undo());
            edit_menu.Append(edit_undo);

            MenuItem edit_redo = new MenuItem("_Redo");
            edit_redo.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.Y, Gdk.ModifierType.ControlMask, AccelFlags.Visible));
            edit_redo.Activated += ((o, e) => UndoStack.Instance.Redo());
            edit_menu.Append(edit_redo);

            // mesh menu
            MenuItem mesh_item = new MenuItem("_Mesh");
            Menu mesh_menu = new Menu();
            mesh_item.Submenu = mesh_menu;
            Append(mesh_item);

            MenuItem mesh_selectAll = new MenuItem("Select _All/None");
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

            mesh_menu.Append(new SeparatorMenuItem());

            MenuItem mesh_smooth = new MenuItem("_Smooth Selected");
            mesh_smooth.AddAccelerator("activate", accelGroup, new AccelKey(Gdk.Key.S, Gdk.ModifierType.None, AccelFlags.Visible));
            mesh_smooth.Activated += ((o, e) => SmoothingManager.Instance.SmoothSelected());
            mesh_menu.Append(mesh_smooth);
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
            Logger.Info("Selecting VRML 2.0 file to open");

            FileChooserDialog fileChooser = new FileChooserDialog(
                "Open Mesh File",
                m_mainWindow,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept
                );

            FileFilter filter = new FileFilter();
            filter.AddPattern("*.wrl");
            filter.Name = "VRML 2.0";
            fileChooser.AddFilter(filter);

            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                // extract the meshes from the file and load them into the scene
                List<Mesh> meshes = VrmlExtractor.Instance.Read(fileChooser.Filename);
                List<MeshInfo> meshInfos = MeshManager.Instance.AddMeshes(meshes);

                // focus on the new meshes if nothing is selected
                if (MeshManager.Instance.SelectedMeshes.Count == 0)
                {
                    Bounds bounds = MainContent.SceneWindow.Meshes.GetBounds(meshInfos);
                    MainContent.SceneWindow.Camera.EaseToBounds(bounds);
                }
            }

            fileChooser.Destroy();
        }

        private void OnQuit(object sender, EventArgs e)
        {
            m_mainWindow.Quit();
        }
    }
}
