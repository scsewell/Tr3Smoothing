using System;
using System.Collections.Generic;
using System.Linq;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Operation for changing the properties of the <see cref="MeshInfo"/> instance
    /// stored by the <see cref="MeshManager"/>.
    /// </summary>
    public class ModifyMeshInfosOperation : Operation
    {
        private readonly List<MeshInfo> m_meshes = new List<MeshInfo>();
        private readonly List<MeshInfo> m_oldSettings = new List<MeshInfo>();
        private readonly List<MeshInfo> m_newSettings = new List<MeshInfo>();
        
        /// <summary>
        /// Creates a new <see cref="ModifyMeshInfosOperation"/> instance.
        /// </summary>
        public ModifyMeshInfosOperation()
        {
            foreach (MeshInfo mesh in MeshManager.Instance.Meshes)
            {
                m_meshes.Add(mesh);
                m_oldSettings.Add(new MeshInfo(mesh));
            }
        }

        /// <summary>
        /// Should be called after modifying all the meshes belonging to this operation.
        /// </summary>
        public void Complete()
        {
            foreach (MeshInfo mesh in m_meshes)
            {
                m_newSettings.Add(new MeshInfo(mesh));
            }

            bool modified = false;
            for (int i = 0; i < m_meshes.Count; i++)
            {
                if (!m_newSettings[i].Equals(m_oldSettings[i]))
                {
                    modified = true;
                    break;
                }
            }

            if (modified)
            {
                Excecute();
                UndoStack.Instance.AddOperation(this);
            }
        }

        /// <summary>
        /// Excecutes the operation.
        /// </summary>
        public override void Excecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
                m_meshes[i].IsActive    = m_newSettings[i].IsActive;
                m_meshes[i].IsSelected  = m_newSettings[i].IsSelected;
                m_meshes[i].IsVisible   = m_newSettings[i].IsVisible;
            }
        }

        /// <summary>
        /// Unexecutes the operation.
        /// </summary>
        public override void Unexcecute()
        {
            for (int i = 0; i < m_meshes.Count; i++)
            {
                m_meshes[i].IsActive    = m_oldSettings[i].IsActive;
                m_meshes[i].IsSelected  = m_oldSettings[i].IsSelected;
                m_meshes[i].IsVisible   = m_oldSettings[i].IsVisible;
            }
        }
    }
}
