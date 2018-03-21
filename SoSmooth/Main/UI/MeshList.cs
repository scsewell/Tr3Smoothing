using System.Collections.Generic;
using System.Linq;
using Gdk;
using Gtk;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Displays the meshes loaded in the scene.
    /// </summary>
    public class MeshList : VBox
    {
        private readonly TreeView m_treeView;
        private readonly ListStore m_listStore;
        private readonly Dictionary<int, MeshInfo> m_indexToMesh = new Dictionary<int, MeshInfo>();
        private readonly Dictionary<MeshInfo, int> m_meshToIndex = new Dictionary<MeshInfo, int>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshList()
        {
            Gtk.Label title = new Gtk.Label();
            title.Markup = "<b>Meshes</b>";

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.HscrollbarPolicy = PolicyType.Never;
            scrolledWindow.VscrollbarPolicy = PolicyType.Automatic;

            Frame frame = new Frame();
            frame.Shadow = ShadowType.EtchedIn;
            frame.Add(scrolledWindow);

            PackStart(title, false, false, 5);
            PackStart(frame, true, true, 0);

            m_treeView = new TreeView();
            m_treeView.HeadersVisible = true;
            m_treeView.EnableSearch = false;
            m_treeView.Selection.Mode = SelectionMode.Multiple;
            scrolledWindow.Add(m_treeView);

            CellRendererToggle cell0 = new CellRendererToggle();
            cell0.Mode = CellRendererMode.Activatable;
            cell0.Sensitive = true;
            cell0.Toggled += OnVisibleToggled;
            TreeViewColumn col0 = new TreeViewColumn();
            col0.Sizing = TreeViewColumnSizing.Fixed;
            col0.FixedWidth = 18;
            col0.PackStart(cell0, false);
            col0.AddAttribute(cell0, "active", 0);
            m_treeView.AppendColumn(col0);

            CellRenderer cell1 = new CellRendererText();
            cell1.Mode = CellRendererMode.Inert;
            TreeViewColumn col1 = new TreeViewColumn();
            col1.Title = "Name";
            col1.PackStart(cell1, true);
            col1.AddAttribute(cell1, "text", 1);
            m_treeView.AppendColumn(col1);
            
            m_listStore = new ListStore(typeof(bool), typeof(string));
            m_treeView.Model = m_listStore;

            m_treeView.Selection.Changed += OnListSelectionChanged;
            m_treeView.QueryTooltip += OnShowTooltip;
            m_treeView.HasTooltip = true;
            m_treeView.CanFocus = false;

            MeshManager.Instance.SelectionChanged += OnMeshSelectionChanged;
            MeshManager.Instance.MeshesChanged += OnMeshesChanged;
            MeshManager.Instance.VisibilityChanged += OnVisibilityChanged;
        }

        /// <summary>
        /// Gets a list of meshes corresponding to the users selection.
        /// </summary>
        private List<MeshInfo> GetSelectedMeshes()
        {
            List<MeshInfo> meshes = new List<MeshInfo>();
            foreach (TreePath path in m_treeView.Selection.GetSelectedRows())
            {
                MeshInfo mesh = m_indexToMesh[path.Indices[0]];
                if (mesh.IsVisible)
                {
                    meshes.Add(mesh);
                }
                else
                {
                    m_treeView.Selection.UnselectPath(path);
                }
            }
            return meshes;
        }

        /// <summary>
        /// Called when the selected elements of the list view has changed.
        /// </summary>
        private void OnListSelectionChanged(object sender, System.EventArgs e)
        {
            List<MeshInfo> selected = GetSelectedMeshes();
            MeshManager.Instance.SetSelectedMeshes(selected);
        }

        /// <summary>
        /// Called when a visibility checkbox has changed.
        /// </summary>
        private void OnVisibleToggled(object o, ToggledArgs args)
        {
            TreePath path = new TreePath(args.Path);
            TreeIter iter;
            if (m_listStore.GetIter(out iter, path))
            {
                bool isVisible = !(bool)m_listStore.GetValue(iter, 0);
                m_listStore.SetValue(iter, 0, isVisible);
                m_indexToMesh[path.Indices[0]].IsVisible = isVisible;
            }
        }

        /// <summary>
        /// Changes the tooltip message based on the cell the mouse is over.
        /// </summary>
        private void OnShowTooltip(object o, QueryTooltipArgs args)
        {
            TreePath path;
            TreeIter iter;
            if (m_treeView.GetPathAtPos(args.X, args.Y, out path) && m_listStore.GetIter(out iter, path))
            {
                int colNum = 0;
                int baseX = 0;
                foreach (TreeViewColumn col in m_treeView.Columns)
                {
                    if (baseX <= args.X && args.X <= baseX + col.Width)
                    {
                        break;
                    }
                    baseX += col.Width;
                    colNum++;
                }
                // visibility column
                if (colNum == 0)
                {
                    bool visible = m_indexToMesh[path.Indices[0] - 1].IsVisible;
                    args.Tooltip.Text = visible ? "Hide mesh in scene." : "Show mesh in scene.";
                    args.RetVal = true;
                }
            }
            if (args.Tooltip != null)
            {
                args.Tooltip.Dispose();
            }
        }

        /// <summary>
        /// Called when the meshes in the scene have changed. Updates the displayed mesh list.
        /// </summary>
        private void OnMeshesChanged(IEnumerable<MeshInfo> meshes)
        {
            m_treeView.Selection.UnselectAll();
            
            m_listStore.Clear();
            m_indexToMesh.Clear();
            m_meshToIndex.Clear();
            
            TreeIter iter = new TreeIter();
            foreach (MeshInfo mesh in meshes)
            {
                iter = m_listStore.AppendValues(mesh.IsVisible, mesh.Mesh.Name);
                TreePath path = m_listStore.GetPath(iter);

                m_indexToMesh.Add(path.Indices[0], mesh);
                m_meshToIndex.Add(mesh, path.Indices[0]);
            }
        }

        /// <summary>
        /// Called when the selected meshes have changed.
        /// </summary>
        private void OnMeshSelectionChanged(IEnumerable<MeshInfo> selected)
        {
            m_treeView.Selection.Changed -= OnListSelectionChanged;
            
            m_treeView.Selection.UnselectAll();
            foreach (MeshInfo mesh in selected)
            {
                TreePath path = new TreePath(m_meshToIndex[mesh].ToString());
                m_treeView.Selection.SelectPath(path);
            }

            m_treeView.Selection.Changed += OnListSelectionChanged;
        }

        /// <summary>
        /// Called when a mesh has had its visibility changed.
        /// </summary>
        private void OnVisibilityChanged(MeshInfo mesh, bool newVisibility)
        {
            m_treeView.Selection.Changed -= OnListSelectionChanged;

            TreePath path = new TreePath(m_meshToIndex[mesh].ToString());
            TreeIter iter;
            m_listStore.GetIter(out iter, path);
            m_listStore.SetValue(iter, 0, newVisibility);

            m_treeView.Selection.UnselectPath(path);

            m_treeView.Selection.Changed += OnListSelectionChanged;
        }
    }
}
