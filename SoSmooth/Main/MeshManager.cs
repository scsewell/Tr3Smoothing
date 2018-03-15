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
        public delegate void MeshesAddedHandler(IEnumerable<Mesh> meshes);
        public delegate void MeshRemovedHandler(Mesh mesh);
        public delegate void MeshesChangedHandler(IEnumerable<Mesh> meshes);
        public delegate void SelctionChangedHandler(IEnumerable<Mesh> selected);
        public delegate void ActiveChangedHandler(Mesh prevActive, Mesh newActive);
        public delegate void VisibilityChangedHandler(Mesh mesh, bool newVisibility);

        /// <summary>
        /// Triggered after a single mesh has been added.
        /// </summary>
        public event MeshesAddedHandler MeshesAdded;

        /// <summary>
        /// Triggered after a single mesh has been removed.
        /// </summary>
        public event MeshRemovedHandler MeshRemoved;

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

        /// <summary>
        /// Triggered when a mesh has has it's visibility changed.
        /// </summary>
        public event VisibilityChangedHandler VisibilityChanged;

        private readonly List<Mesh> m_meshes = new List<Mesh>();
        private readonly List<Mesh> m_selected = new List<Mesh>();
        private readonly List<Mesh> m_visible = new List<Mesh>();
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
        /// The visible meshes.
        /// </summary>
        public IReadOnlyList<Mesh> Visible => m_visible;

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
            m_meshes.AddRange(meshes);
            m_visible.AddRange(meshes);
            MeshesAdded?.Invoke(meshes);
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
                // remove from selection
                m_selected.Remove(mesh);
                // notify the mesh's removal
                if (m_meshes.Remove(mesh))
                {
                    MeshRemoved?.Invoke(mesh);
                }
            }

            // notify if the selection has changed
            if (selectedCount != m_selected.Count)
            {
                SelectionChanged?.Invoke(m_selected);
            }

            // notify that there are new meshes
            MeshesChanged?.Invoke(m_meshes);
            
            // dispose old meshes to prevent memory leak of unmanaged mesh data
            foreach (Mesh mesh in meshes)
            {
                m_visible.Remove(mesh);
                mesh.Dispose();
            }
        }

        /// <summary>
        /// Sets the active mesh.
        /// </summary>
        /// <param name="mesh">The mesh to make active.</param>
        /// <param name="clearSelection">If true the active mesh replaces the selection.</param>
        public void SetActiveMesh(Mesh mesh, bool clearSelection)
        {
            // clear the former selection if desired
            bool selectionChanged = false;
            if (clearSelection && m_selected.Count > 0)
            {
                m_selected.Clear();
                selectionChanged = true;
            }

            // The active mesh must also be selected
            if (mesh != null && !m_selected.Contains(mesh))
            {
                m_selected.Add(mesh);
                selectionChanged = true;
            }

            // notify any selection changes
            if (selectionChanged)
            {
                SelectionChanged?.Invoke(m_selected);
            }

            // notify that there is a new active mesh
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
            if (meshes != null)
            {
                // clear any old selection
                SetActiveMesh(null, true);

                // add the new selection
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

                // notify the selection change
                SelectionChanged?.Invoke(m_selected);

                // make the first selected mesh active
                if (m_selected.Count > 0)
                {
                    SetActiveMesh(m_selected.First(), false);
                }
            }
            else
            {
                SetActiveMesh(null, true);
            }
        }
        
        /// <summary>
        /// Gets is a mesh is may be shown in the scene.
        /// </summary>
        /// <param name="mesh">The mesh to get the visiblility of.</param>
        /// <returns>True if the mesh may visible in the scene.</returns>
        public bool IsVisible(Mesh mesh)
        {
            return m_visible.Contains(mesh);
        }
        
        /// <summary>
        /// Sets if a mesh may be visible in the scene.
        /// </summary>
        /// <param name="mesh">The mesh to set the visibility of.</param>
        /// <param name="visibility">Whether or not this mesh may be visible.</param>
        public void SetVisible(Mesh mesh, bool visibility)
        {
            if (IsVisible(mesh) != visibility)
            {
                if (visibility)
                {
                    m_visible.Add(mesh);
                }
                else
                { 
                    m_visible.Remove(mesh);
                }

                // if active make inactive
                if (mesh == m_active)
                {
                    SetActiveMesh(null, false);
                }

                // if selected, remove from selection and notify the change
                if (m_selected.Remove(mesh))
                {
                    SelectionChanged?.Invoke(m_selected);
                }

                // notify the new visibility
                VisibilityChanged?.Invoke(mesh, visibility);
            }
        }

        /// <summary>
        /// Delects all meshes if any are selected and selects all meshes is none are selected.
        /// </summary>
        public void ToggleSelected()
        {
            if (m_selected.Count > 0)
            {
                SetSelectedMeshes(null);
            }
            else
            {
                SetSelectedMeshes(m_visible);
            }
        }

        /// <summary>
        /// Removes all selected meshes.
        /// </summary>
        public void DeleteSelected()
        {
            List<Mesh> selected = new List<Mesh>(m_selected);
            RemoveMeshes(selected);
        }

        /// <summary>
        /// Makes every mesh visible.
        /// </summary>
        public void ShowAll()
        {
            List<Mesh> notVisible = new List<Mesh>(m_meshes);
            foreach (Mesh mesh in m_visible)
            {
                notVisible.Remove(mesh);
            }

            foreach (Mesh mesh in notVisible)
            {
                SetVisible(mesh, true);
            }

            if (notVisible.Count > 0)
            {
                SetSelectedMeshes(notVisible);
            }
        }

        /// <summary>
        /// Makes every mesh visible.
        /// </summary>
        public void HideSelected()
        {
            List<Mesh> selected = new List<Mesh>(m_selected);
            foreach (Mesh mesh in selected)
            {
                SetVisible(mesh, false);
            }
        }
    }
}
