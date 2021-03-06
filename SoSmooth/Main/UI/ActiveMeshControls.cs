﻿using System;
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
            HeightRequest = 150;

            Label title = new Label();
            title.Markup = "<b>Active Mesh</b>";
            PackStart(title, false, false, 5);

            m_name = CreateField("Name", "The name of the mesh.");
            m_vertexCount = CreateField("Vertices", "The number of vertices in the mesh.");
            m_triangleCount = CreateField("Triangles", "The number of triangles in the mesh.");

            m_hiddenOnNoActive.ForEach(w => w.HideAll());

            Shown += OnShown;

            MeshManager.Instance.MeshesAdded += OnMeshesAdded;
            MeshManager.Instance.MeshRemoved += OnMeshRemoved;
        }

        /// <summary>
        /// Called when meshes are added to the scene.
        /// </summary>
        private void OnMeshesAdded(IEnumerable<MeshInfo> meshes)
        {
            foreach (MeshInfo mesh in meshes)
            {
                mesh.ActiveChanged += SetDisplayedMesh;
            }
        }

        /// <summary>
        /// Called when a mesh is added to the scene.
        /// </summary>
        private void OnMeshRemoved(MeshInfo mesh)
        {
            mesh.ActiveChanged -= SetDisplayedMesh;
        }

        /// <summary>
        /// Hide the contents if there is no active mesh.
        /// </summary>
        private void OnShown(object sender, EventArgs e)
        {
            SetDisplayedMesh(MeshManager.Instance.ActiveMesh);
        }

        /// <summary>
        /// Called when the active mesh in the user's selection has changed.
        /// </summary>
        /// <param name="prevActive">The mesh.</param>
        private void SetDisplayedMesh(MeshInfo mesh)
        {
            if (mesh != null && mesh.IsActive)
            {
                m_name.Text = mesh.Mesh.Name;
                m_vertexCount.Text = mesh.Mesh.VertexCount.ToString();
                m_triangleCount.Text = mesh.Mesh.TriangleCount.ToString();

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
            HBox box = new HBox();
            if (tooltip != null)
            {
                box.TooltipText = tooltip;
            }

            Label titleLabel = new Label(title + ":");
            titleLabel.WidthRequest = 60;
            titleLabel.Xalign = 0;

            Label valueLabel = new Label();
            valueLabel.Xalign = 0;
            
            box.PackStart(titleLabel, false, false, 5);
            box.PackStart(valueLabel, true, true, 5);
            PackStart(box, false, false, 1);

            m_hiddenOnNoActive.Add(box);
            
            return valueLabel;
        }
    }
}
