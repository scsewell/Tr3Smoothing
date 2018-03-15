using System;
using System.Collections.Generic;
using System.Linq;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Manages the meshes loaded in the scene.
    /// </summary>
    public class MeshManager : Singleton<MeshManager>
    {
        public delegate void MeshAddedHandler(Mesh mesh);
        public delegate void MeshRemovedHandler(Mesh mesh);
        public delegate void MeshesChangedHandler(IReadOnlyList<Mesh> meshes);
        public delegate void SelctionChangedHandler(IReadOnlyList<Mesh> selected);
        public delegate void ActiveChangedHandler(Mesh prevActive, Mesh newActive);

        /// <summary>
        /// Triggered after a single mesh has been added.
        /// </summary>
        public event MeshAddedHandler MeshAdded;

        /// <summary>
        /// Triggered after a single mesh has been removed.
        /// </summary>
        public event MeshAddedHandler MeshRemoved;

        /// <summary>
        /// Triggered after meshes have been added or removed.
        /// </summary>
        public event MeshesChangedHandler MeshesChanged;
        
        /// <summary>
        /// Triggered when the selected meshes have changed.
        /// </summary>
        public event SelctionChangedHandler SelectionChanged;
        
        /// <summary>
        /// Triggered when the active mesh has been changed.
        /// </summary>
        public event ActiveChangedHandler ActiveChanged;

        private readonly List<Mesh> m_meshes = new List<Mesh>();
        private readonly List<Mesh> m_selected = new List<Mesh>();
        private Mesh m_active;

        /// <summary>
        /// The loaded meshes.
        /// </summary>
        public IReadOnlyList<Mesh> Meshes => m_meshes;

        /// <summary>
        /// The selected meshes.
        /// </summary>
        public IReadOnlyList<Mesh> Selected => m_selected;

        /// <summary>
        /// The active mesh.
        /// </summary>
        public Mesh Active => m_active;

        /// <summary>
        /// Adds meshes to the loaded mesh list.
        /// </summary>
        /// <param name="meshes">The meshes to add.</param>
        public void AddMeshes(IEnumerable<Mesh> meshes)
        {
            foreach (Mesh mesh in meshes)
            {
                m_meshes.Add(mesh);
                MeshAdded?.Invoke(mesh);
            }
            MeshesChanged?.Invoke(m_meshes);
        }

        /// <summary>
        /// Removes a mesh from the loaded mesh list.
        /// </summary>
        /// <param name="meshes">The meshes to remove.</param>
        public void RemoveMeshes(IEnumerable<Mesh> meshes)
        {
            int selectedCount = m_selected.Count;

            foreach (Mesh mesh in meshes)
            {
                // If the mesh is active or selected notify its removal
                if (Active == mesh)
                {
                    SetActiveMesh(null, false);
                }
                m_selected.Remove(mesh);
                if (m_meshes.Remove(mesh))
                {
                    MeshRemoved?.Invoke(mesh);
                }
                mesh.Dispose();
            }

            if (selectedCount != m_selected.Count)
            {
                SelectionChanged?.Invoke(m_selected);
            }

            MeshesChanged?.Invoke(m_meshes);
        }

        /// <summary>
        /// Sets the active mesh.
        /// </summary>
        /// <param name="mesh">The mesh to make active.</param>
        /// <param name="clearSelection">If true the active mesh replaces the selection.</param>
        public void SetActiveMesh(Mesh mesh, bool clearSelection)
        {
            bool selectionChanged = false;
            if (clearSelection && m_selected.Count > 0)
            {
                m_selected.Clear();
                selectionChanged = true;
            }

            if (mesh != null && !m_selected.Contains(mesh))
            {
                m_selected.Add(mesh);
                selectionChanged = true;
            }

            if (selectionChanged)
            {
                SelectionChanged?.Invoke(m_selected);
            }

            if (m_active != mesh)
            {
                Mesh previous = m_active;
                m_active = mesh;
                
                ActiveChanged?.Invoke(previous, m_active);
            }
        }

        /// <summary>
        /// Sets the selected meshes.
        /// </summary>
        /// <param name="mesh">The mesh to select.</param>
        public void SetSelectedMeshes(IEnumerable<Mesh> meshes)
        {
            m_selected.Clear();

            if (meshes != null)
            {
                foreach (Mesh mesh in meshes)
                {
                    if (mesh != null)
                    {
                        m_selected.Add(mesh);
                    }
                }
            }

            SelectionChanged?.Invoke(m_selected);
        }
    }
}
