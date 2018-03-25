using System;
using System.Collections.Generic;
using System.Linq;

namespace SoSmooth
{
    /// <summary>
    /// Operation for changing the properties of the <see cref="MeshInfo"/> instance
    /// stored by the <see cref="MeshManager"/>.
    /// </summary>
    public class ModifyMeshInfosOperation : Operation
    {
        private readonly List<MeshInfo> m_oldMeshes = new List<MeshInfo>();
        private readonly List<MeshInfo> m_newMeshes = new List<MeshInfo>();

        private readonly List<MeshInfo> m_added = new List<MeshInfo>();
        private readonly List<MeshInfo> m_removed = new List<MeshInfo>();

        private readonly List<MeshInfo> m_oldSettings = new List<MeshInfo>();
        private readonly List<MeshInfo> m_newSettings = new List<MeshInfo>();
        
        /// <summary>
        /// Creates a new <see cref="ModifyMeshInfosOperation"/> instance.
        /// </summary>
        public ModifyMeshInfosOperation()
        {
            // remember the state of meshes before the modifications
            m_oldMeshes.AddRange(MeshManager.Instance.Meshes);
            m_oldMeshes.ForEach(m => m_oldSettings.Add(new MeshInfo(m)));
        }

        /// <summary>
        /// Should be called after modifying all the meshes belonging to this operation.
        /// </summary>
        public void Complete()
        {
            m_newMeshes.AddRange(MeshManager.Instance.Meshes);
            m_newMeshes.ForEach(m => m_newSettings.Add(new MeshInfo(m)));

            m_added.AddRange(m_newMeshes.Where(m => !m_oldMeshes.Contains(m)));
            m_removed.AddRange(m_oldMeshes.Where(m => !m_newMeshes.Contains(m)));

            // Only add this operation to the stack if meshes were added,
            // removed, or had their properties changed.
            bool modified = m_added.Count > 0 || m_removed.Count > 0;

            if (!modified)
            {
                for (int i = 0; i < m_newMeshes.Count; i++)
                {
                    if (!m_newSettings[i].Equals(m_oldSettings[i]))
                    {
                        modified = true;
                        break;
                    }
                }
            }

            if (modified)
            {
                UndoStack.Instance.AddOperation(this);
            }
        }

        /// <summary>
        /// Excecutes the operation.
        /// </summary>
        public override void Excecute()
        {
            // add any new meshes
            MeshManager.Instance.AddMeshes(m_added);

            // remove meshes that were deleted
            MeshManager.Instance.RemoveMeshes(m_removed);

            // apply the new settings
            for (int i = 0; i < m_newMeshes.Count; i++)
            {
                m_newMeshes[i].IsActive    = m_newSettings[i].IsActive;
                m_newMeshes[i].IsSelected  = m_newSettings[i].IsSelected;
                m_newMeshes[i].IsVisible   = m_newSettings[i].IsVisible;
            }
        }

        /// <summary>
        /// Unexecutes the operation.
        /// </summary>
        public override void Unexcecute()
        {
            // remove and meshes that were added
            MeshManager.Instance.RemoveMeshes(m_added);

            // add back any meshes that were removed
            MeshManager.Instance.AddMeshes(m_removed);

            // apply the original settings for the meshes
            for (int i = 0; i < m_oldMeshes.Count; i++)
            {
                m_oldMeshes[i].IsActive    = m_oldSettings[i].IsActive;
                m_oldMeshes[i].IsSelected  = m_oldSettings[i].IsSelected;
                m_oldMeshes[i].IsVisible   = m_oldSettings[i].IsVisible;
            }
        }

        /// <summary>
        /// Disposes meshes that have been removed once the operation can no longer be undone.
        /// </summary>
        public override void OnUndoDropped()
        {
            foreach (MeshInfo mesh in m_removed)
            {
                mesh.Mesh.Dispose();
            }
        }
    }
}
