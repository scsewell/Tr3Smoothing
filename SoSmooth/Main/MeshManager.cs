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
        public delegate void MeshesAddedHandler(IEnumerable<MeshInfo> meshes);
        public delegate void MeshRemovedHandler(MeshInfo mesh);
        public delegate void MeshesChangedHandler(IEnumerable<MeshInfo> meshes);
        public delegate void SelctionChangedHandler(IEnumerable<MeshInfo> selected);
        public delegate void ActiveChangedHandler(MeshInfo prevActive, MeshInfo newActive);
        public delegate void VisibilityChangedHandler(MeshInfo mesh, bool newVisibility);

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

        private readonly List<MeshInfo> m_meshes = new List<MeshInfo>();
        
        /// <summary>
        /// The loaded meshes.
        /// </summary>
        public List<MeshInfo> Meshes => new List<MeshInfo>(m_meshes);

        /// <summary>
        /// The selected meshes.
        /// </summary>
        public List<MeshInfo> SelectedMeshes => m_meshes.Where(m => m.IsSelected).ToList();

        /// <summary>
        /// The visible meshes.
        /// </summary>
        public List<MeshInfo> VisibleMeshes => m_meshes.Where(m => m.IsVisible).ToList();

        /// <summary>
        /// The active mesh.
        /// </summary>
        public MeshInfo ActiveMesh => m_meshes.FirstOrDefault(m => m.IsActive);

        /// <summary>
        /// Adds meshes to the loaded mesh list.
        /// </summary>
        /// <param name="meshes">The meshes to add.</param>
        public void AddMeshes(IEnumerable<Mesh> meshes)
        {
            List<MeshInfo> newMeshes = new List<MeshInfo>();
            foreach (Mesh mesh in meshes)
            {
                newMeshes.Add(new MeshInfo(mesh));
            }
            MeshesAdded?.Invoke(newMeshes);

            m_meshes.AddRange(newMeshes);
            MeshesChanged?.Invoke(m_meshes);
        }

        /// <summary>
        /// Removes a mesh from the loaded mesh list.
        /// </summary>
        /// <param name="meshes">The meshes to remove.</param>
        public void RemoveMeshes(IEnumerable<MeshInfo> meshes)
        {
            MeshInfo previousActive = null;
            bool selectionChanged = false;

            // remove meshes from selection
            foreach (MeshInfo mesh in meshes)
            {
                if (mesh.IsActive)
                {
                    mesh.IsActive = false;
                    previousActive = mesh;
                }

                if (mesh.IsSelected)
                {
                    mesh.IsSelected = false;
                    selectionChanged = true;
                }
            }

            // notify if the selection has changed
            if (previousActive != null)
            {
                ActiveChanged?.Invoke(previousActive, null);
            }

            // notify if the selection has changed
            if (selectionChanged)
            {
                SelectionChanged?.Invoke(SelectedMeshes);
            }

            // notify that there are new meshes
            MeshesChanged?.Invoke(m_meshes);

            // notify each mesh's removal
            foreach (MeshInfo mesh in meshes)
            {
                MeshRemoved?.Invoke(mesh);
                // dispose old meshes to prevent memory leak of unmanaged mesh data
                mesh.Mesh.Dispose();
            }
        }

        /// <summary>
        /// Sets the active mesh.
        /// </summary>
        /// <param name="mesh">The mesh to make active.</param>
        /// <param name="clearSelection">If true the active mesh replaces the selection.</param>
        public void SetActiveMesh(MeshInfo mesh, bool clearSelection)
        {
            MeshInfo previousActive = null;
            bool selectionChanged = false;

            // find an clear the previously active mesh
            foreach (MeshInfo meshInfo in m_meshes)
            {
                if (meshInfo.IsActive)
                {
                    previousActive = meshInfo;
                    meshInfo.IsActive = false;
                }

                // clear the former selection if desired
                if (clearSelection && meshInfo.IsSelected)
                {
                    meshInfo.IsSelected = false;
                    selectionChanged = true;
                }
            }

            // The new active mesh must also be selected
            if (mesh != null)
            {
                mesh.IsActive = true;
                if (!mesh.IsSelected)
                {
                    mesh.IsSelected = true;
                    selectionChanged = true;
                }
            }

            // notify any selection changes
            if (selectionChanged)
            {
                SelectionChanged?.Invoke(SelectedMeshes);
            }

            // notify that there is a new active mesh
            if (previousActive != mesh)
            {
                ActiveChanged?.Invoke(previousActive, mesh);
            }
        }

        /// <summary>
        /// Sets the selected meshes.
        /// </summary>
        /// <param name="mesh">The mesh to select.</param>
        public void SetSelectedMeshes(IEnumerable<MeshInfo> meshes)
        {
            if (meshes != null)
            {
                // set the new selection
                foreach (MeshInfo meshInfo in m_meshes)
                {
                    meshInfo.IsActive = false;
                    meshInfo.IsSelected = false;
                }
                foreach (MeshInfo meshInfo in meshes)
                {
                    meshInfo.IsSelected = true;
                }

                // notify the selection change
                SelectionChanged?.Invoke(SelectedMeshes);

                // make the first selected mesh active
                SetActiveMesh(SelectedMeshes.FirstOrDefault(), false);
            }
            else
            {
                SetActiveMesh(null, true);
            }
        }
        
        /// <summary>
        /// Delects all meshes if any are selected and selects all meshes is none are selected.
        /// </summary>
        public void ToggleSelected()
        {
            if (m_meshes.Any(m => m.IsSelected))
            {
                SetSelectedMeshes(null);
            }
            else
            {
                SetSelectedMeshes(VisibleMeshes);
            }
        }

        /// <summary>
        /// Removes all selected meshes.
        /// </summary>
        public void DeleteSelected()
        {
            RemoveMeshes(SelectedMeshes);
        }

        /// <summary>
        /// Makes every mesh visible.
        /// </summary>
        public void ShowAll()
        {
            List<MeshInfo> notVisible = m_meshes.Where(m => !m.IsVisible).ToList();
            foreach (MeshInfo mesh in notVisible)
            {
                mesh.IsVisible = true;
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
            foreach (MeshInfo mesh in m_meshes)
            {
                if (mesh.IsSelected)
                {
                    mesh.IsVisible = false;
                }
            }
        }
    }
}
