using System;
using System.Collections.Generic;
using Gtk;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// A panel that contains controls relating to the active mesh that can be operated upon.
    /// </summary>
    public class ActiveMeshControls : VBox
    {
        private Label m_name;
        private Label m_vertexCount;
        private Label m_triangleCount;

        private List<Widget> m_hiddenOnNoActive = new List<Widget>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActiveMeshControls()
        {
            HeightRequest = 200;

            Gtk.Label title = new Gtk.Label();
            title.Markup = "<b>Active Mesh</b>";
            title.TooltipText = "The mesh most recently selected.";
            PackStart(title, false, false, 5);

            m_name = CreateField("Name", "The name of the mesh.");
            m_vertexCount = CreateField("Vertices", "The number of vertices in the mesh.");
            m_triangleCount = CreateField("Triangles", "The number of triangles in the mesh.");

            m_hiddenOnNoActive.ForEach(w => w.HideAll());

            Shown += OnShown;

            MeshManager.Instance.ActiveChanged += OnActiveMeshChanged;
        }

        /// <summary>
        /// Hide the contents if there is no active mesh
        /// </summary>
        private void OnShown(object sender, EventArgs e)
        {
            bool hasActive = MeshManager.Instance.ActiveMesh != null;
            if (hasActive)
            {
                m_hiddenOnNoActive.ForEach(w => w.ShowAll());
            }
            else
            {
                m_hiddenOnNoActive.ForEach(w => w.HideAll());
            }
        }

        /// <summary>
        /// Called when the active mesh in the user's selection has changed.
        /// </summary>
        /// <param name="prevActive">The previously active mesh.</param>
        /// <param name="newActive">The currently active mesh.</param>
        private void OnActiveMeshChanged(MeshInfo prevActive, MeshInfo newActive)
        {
            if (newActive != null)
            {
                Mesh mesh = newActive.Mesh;

                m_name.Text = mesh.Name;
                m_vertexCount.Text = mesh.VertexCount.ToString();
                m_triangleCount.Text = mesh.TriangleCount.ToString();

                m_hiddenOnNoActive.ForEach(w => w.ShowAll());
            }
            else
            {
                m_hiddenOnNoActive.ForEach(w => w.HideAll());
            }
        }

        /// <summary>
        /// Creates a label field with a title label and value label.
        /// </summary>
        /// <param name="title">The title of the field.</param>
        /// <param name="tooltip">The tooltip displayed at the title.</param>
        /// <returns>The label where the value is displayed.</returns>
        private Label CreateField(string title, string tooltip = null)
        {
            Label titleLabel = new Label(title + ":");
            if (tooltip != null)
            {
                titleLabel.TooltipText = tooltip;
            }
            titleLabel.WidthRequest = 60;
            titleLabel.Xalign = 0;

            Label valueLabel = new Label();
            valueLabel.Xalign = 0;

            HBox box = new HBox();
            box.PackStart(titleLabel, false, false, 5);
            box.PackStart(valueLabel, true, true, 5);
            PackStart(box, false, false, 1);

            m_hiddenOnNoActive.Add(box);
            
            return valueLabel;
        }
    }
}
