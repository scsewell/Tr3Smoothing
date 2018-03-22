using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
using SoSmooth.Meshes;

namespace SoSmooth
{
    /// <summary>
    /// Stores meta information about loaded meshes in the scene.
    /// </summary>
    public class MeshInfo
    {
        public delegate void VisibilityChangedHandler(MeshInfo mesh);
        public delegate void SelctionChangedHandler(MeshInfo mesh);
        public delegate void ActiveChangedHandler(MeshInfo mesh);
        
        /// <summary>
        /// Triggered when a mesh has has it's visibility changed.
        /// </summary>
        public event VisibilityChangedHandler VisibilityChanged;

        /// <summary>
        /// Triggered when the selected meshes have changed.
        /// </summary>
        public event SelctionChangedHandler SelectionChanged;

        /// <summary>
        /// Triggered when the active mesh has been changed.
        /// </summary>
        public event ActiveChangedHandler ActiveChanged;

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
                if (m_visible != value)
                {
                    m_visible = value;
                    // hidden mesh is never selected
                    if (!m_visible)
                    {
                        IsSelected = false;
                    }
                    VisibilityChanged?.Invoke(this);
                }
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
                if (m_selected != value)
                {
                    m_selected = value;
                    // unselected mesh is never active
                    if (!m_selected)
                    {
                        IsActive = false;
                    }
                    SelectionChanged?.Invoke(this);
                }
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
                if (m_active != value)
                {
                    m_active = value;
                    // active mesh is also selected
                    if (m_active)
                    {
                        IsSelected = true;
                    }
                    ActiveChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="MeshInfo"/> instance.
        /// </summary>
        /// <param name="mesh">The mesh to create meta information for.</param>
        public MeshInfo(Mesh mesh)
        {
            m_mesh      = mesh;
            m_color     = Random.GetColor();
            m_visible   = true;
            m_selected  = false;
            m_active    = false;
        }

        /// <summary>
        /// Clones a <see cref="MeshInfo"/> instance.
        /// </summary>
        /// <param name="info">The instance to clone.</param>
        public MeshInfo(MeshInfo info)
        {
            m_mesh      = info.m_mesh;
            m_color     = info.m_color;
            m_visible   = info.m_visible;
            m_selected  = info.m_selected;
            m_active    = info.m_active;
        }

        /// <summary>
        /// Compares the values of this instance against another
        /// <see cref="MeshInfo"/> instance.
        /// </summary>
        public bool Equals(MeshInfo info)
        {
            return
                Mesh        == info.Mesh &&
                Color       == info.Color &&
                IsVisible   == info.IsVisible &&
                IsSelected  == info.IsSelected &&
                IsActive    == IsActive;
        }

        /// <summary>
        /// Creates a nicely formatted string representing the instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{{Mesh.Name} Visible:{IsVisible} Selected:{IsSelected} Active:{IsActive}}}";
        }
    }
}
