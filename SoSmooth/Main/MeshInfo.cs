using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Stores meta information about loaded meshes.
    /// </summary>
    public class MeshInfo
    {
        private readonly Mesh m_mesh;
        private readonly Color4 m_color;
        private bool m_visible;
        private bool m_selected;
        private bool m_active;

        /// <summary>
        /// The mesh associated with this instance.
        /// </summary>
        public Mesh Mesh => m_mesh;

        /// <summary>
        /// The color used for the mesh.
        /// </summary>
        public Color4 Color => m_color;

        /// <summary>
        /// If true the mesh can be seen in the scene window.
        /// </summary>
        public bool IsVisible
        {
            get { return m_visible; }
            set
            {
                m_visible = value;
            }
        }

        /// <summary>
        /// If true the mesh is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get { return m_selected; }
            set
            {
                m_selected = value;
            }
        }

        /// <summary>
        /// If true the mesh is the currently active mesh.
        /// </summary>
        public bool IsActive
        {
            get { return m_active; }
            set
            {
                m_active = value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="MeshInfo"/> instance.
        /// </summary>
        /// <param name="mesh">The mesh to create meta information for.</param>
        public MeshInfo(Mesh mesh)
        {
            m_mesh = mesh;
            m_color = Random.GetColor();
            m_visible = true;
            m_selected = false;
            m_active = false;
        }
    }
}
