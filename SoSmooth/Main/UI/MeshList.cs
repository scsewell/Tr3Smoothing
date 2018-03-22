using System.Collections.Generic;
using System.Linq;
using Gtk;

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
            Label title = new Label();
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

            MeshManager.Instance.MeshesAdded += OnMeshesAdded;
            MeshManager.Instance.MeshRemoved += OnMeshRemoved;
            MeshManager.Instance.MeshesChanged += OnMeshesChanged;
        }

        /// <summary>
        /// Called when meshes are added to the scene.
        /// </summary>
        private void OnMeshesAdded(IEnumerable<MeshInfo> meshes)
        {
            foreach (MeshInfo mesh in meshes)
            {
                mesh.SelectionChanged += OnMeshSelectionChanged;
                mesh.VisibilityChanged += OnVisibilityChanged;
            }
        }

        /// <summary>
        /// Called when a mesh is added to the scene.
        /// </summary>
        private void OnMeshRemoved(MeshInfo mesh)
        {
            mesh.SelectionChanged -= OnMeshSelectionChanged;
            mesh.VisibilityChanged -= OnVisibilityChanged;
        }

        /// <summary>
        /// Called when the meshes in the scene have changed. Updates the displayed mesh list.
        /// </summary>
        private void OnMeshesChanged(IEnumerable<MeshInfo> meshes)
        {
            m_treeView.Selection.Changed -= OnListSelectionChanged;

            m_listStore.Clear();
            m_indexToMesh.Clear();
            m_meshToIndex.Clear();

            TreeIter iter = new TreeIter();
            foreach (MeshInfo mesh in meshes)
            {
                iter = m_listStore.AppendValues(mesh.IsVisible, mesh.Mesh.Name);
                TreePath path = m_listStore.GetPath(iter);

                if (mesh.IsSelected)
                {
                    m_treeView.Selection.SelectPath(path);
                }

                m_indexToMesh.Add(path.Indices[0], mesh);
                m_meshToIndex.Add(mesh, path.Indices[0]);
            }

            m_treeView.Selection.Changed += OnListSelectionChanged;
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

        private bool m_ignoreListSelection = false;

        /// <summary>
        /// Called when the selected elements of the list view has changed.
        /// </summary>
        private void OnListSelectionChanged(object sender, System.EventArgs e)
        {
            if (!m_ignoreListSelection)
            {
                ModifyMeshInfosOperation op = new ModifyMeshInfosOperation();

                // deselect old selection
                List<MeshInfo> oldSelection = MeshManager.Instance.SelectedMeshes;
                oldSelection.ForEach(m => m.IsSelected = false);

                List<MeshInfo> newSelected = GetSelectedMeshes().Where(m => !oldSelection.Contains(m)).ToList();
                newSelected.ForEach(m => m.IsSelected = true);

                if (newSelected.Count > 0)
                {
                    if (oldSelection.Count > 0 && m_meshToIndex[oldSelection.First()] < m_meshToIndex[newSelected.First()])
                    {
                        newSelected.Last().IsActive = true;
                    }
                    else
                    {
                        newSelected.First().IsActive = true;
                    }
                }

                op.Complete();
            }
            else
            {
                m_treeView.Selection.Changed -= OnListSelectionChanged;

                m_treeView.Selection.UnselectAll();
                foreach (KeyValuePair<MeshInfo, int> meshPath in m_meshToIndex)
                {
                    if (meshPath.Key.IsSelected)
                    {
                        TreePath path = new TreePath(meshPath.Value.ToString());
                        m_treeView.Selection.SelectPath(path);
                    }
                }
                m_ignoreListSelection = false;

                m_treeView.Selection.Changed += OnListSelectionChanged;
            }
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
                ModifyMeshInfosOperation op = new ModifyMeshInfosOperation();
                m_ignoreListSelection = true;
                m_indexToMesh[path.Indices[0]].IsVisible = !(bool)m_listStore.GetValue(iter, 0);
                op.Complete();
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
        /// Called when the selected meshes have changed.
        /// </summary>
        private void OnMeshSelectionChanged(MeshInfo mesh)
        {
            m_treeView.Selection.Changed -= OnListSelectionChanged;
            
            TreePath path = new TreePath(m_meshToIndex[mesh].ToString());
            if (mesh.IsSelected)
            {
                m_treeView.Selection.SelectPath(path);
            }
            else
            {
                m_treeView.Selection.UnselectPath(path);
            }

            m_treeView.Selection.Changed += OnListSelectionChanged;
        }

        /// <summary>
        /// Called when a mesh has had its visibility changed.
        /// </summary>
        private void OnVisibilityChanged(MeshInfo mesh)
        {
            m_treeView.Selection.Changed -= OnListSelectionChanged;

            TreePath path = new TreePath(m_meshToIndex[mesh].ToString());
            TreeIter iter;
            if (m_listStore.GetIter(out iter, path))
            {
                m_listStore.SetValue(iter, 0, mesh.IsVisible);
                m_treeView.Selection.UnselectPath(path);
            }
            
            m_treeView.Selection.Changed += OnListSelectionChanged;
        }
    }
}
