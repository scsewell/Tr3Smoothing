using System.Collections.Generic;
using System.Linq;
using Gtk;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Displays the meshes loaded in the scene.
    /// </summary>
    public class MeshList : ScrolledWindow
    {
        private readonly TreeView m_treeView;
        private readonly ListStore m_listStore;
        private readonly Dictionary<int, Mesh> m_indexToMesh = new Dictionary<int, Mesh>();
        private readonly Dictionary<Mesh, int> m_meshToIndex = new Dictionary<Mesh, int>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MeshList()
        {
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Automatic;

            m_treeView = new TreeView();
            m_treeView.HeadersVisible = true;
            m_treeView.EnableSearch = false;
            m_treeView.Selection.Mode = SelectionMode.Multiple;
            Add(m_treeView);
            
            TreeViewColumn col1 = new TreeViewColumn();
            CellRenderer cell1 = new CellRendererText();
            col1.Title = "Mesh";
            col1.PackStart(cell1, true);
            col1.AddAttribute(cell1, "text", 0);
            m_treeView.AppendColumn(col1);
            /*
            TreeViewColumn col2 = new TreeViewColumn();
            CellRenderer cell2 = new cellrenderer();
            col2.Title = "Remove";
            col2.PackStart(cell2, true);
            col2.AddAttribute(cell2, "text", 1);
            AppendColumn(col2);
            */
            m_listStore = new ListStore(typeof(string));
            m_treeView.Model = m_listStore;

            m_treeView.Selection.Changed += OnListSelectionChanged;
            MeshManager.Instance.SelectionChanged += OnMeshSelectionChanged;
            MeshManager.Instance.MeshesChanged += OnMeshesChanged;
        }

        /// <summary>
        /// Called when the meshes in the scene have changed. Updates the displayed mesh list.
        /// </summary>
        private void OnMeshesChanged(IReadOnlyList<Mesh> meshes)
        {
            m_treeView.Selection.UnselectAll();
            
            m_listStore.Clear();
            m_indexToMesh.Clear();
            m_meshToIndex.Clear();

            TreeIter iter = new TreeIter();
            for (int i = 0; i < meshes.Count; i++)
            {
                iter = m_listStore.AppendValues(meshes[i].Name);
                m_indexToMesh.Add(i, meshes[i]);
                m_meshToIndex.Add(meshes[i], i);
            }
        }

        /// <summary>
        /// Called when the selected meshes have changed.
        /// </summary>
        private void OnMeshSelectionChanged(IReadOnlyList<Mesh> selected)
        {
            m_treeView.Selection.Changed -= OnListSelectionChanged;
            
            m_treeView.Selection.UnselectAll();
            foreach (Mesh mesh in selected)
            {
                m_treeView.Selection.SelectPath(new TreePath($"{m_meshToIndex[mesh]}"));
            }

            m_treeView.Selection.Changed += OnListSelectionChanged;
        }
        
        /// <summary>
        /// Called when the selected elements of the list view has changed.
        /// </summary>
        private void OnListSelectionChanged(object sender, System.EventArgs e)
        {
            List<Mesh> selected = GetSelectedMeshes();
            MeshManager.Instance.SetSelectedMeshes(selected);
            MeshManager.Instance.SetActiveMesh(selected.Count > 0 ? selected.First() : null, false);
        }
        
        /// <summary>
        /// Called when key is pressed while focused.
        /// </summary>
        public void OnKeyPress(object o, KeyPressEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Delete)
            {
                MeshManager.Instance.RemoveMeshes(GetSelectedMeshes());
            }
        }

        /// <summary>
        /// Gets a list of meshes corresponding to the users selection.
        /// </summary>
        private List<Mesh> GetSelectedMeshes()
        {
            List<Mesh> meshes = new List<Mesh>();
            foreach (TreePath path in m_treeView.Selection.GetSelectedRows())
            {
                meshes.Add(m_indexToMesh[path.Indices[0]]);
            }
            return meshes;
        }
    }
}
