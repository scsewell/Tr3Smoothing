using System;
using System.Collections.Generic;
using OpenTK;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Stores spatial orientation and scenegraph heirarchy data. All entities must
    /// have a transform.
    /// </summary>
    public sealed class Transform : Component
    {
        // cached values
        private Vector3 m_localPosition;
        private Quaternion m_localRotation;
        private Vector3 m_localScale;

        private Vector3 m_forward;
        private Vector3 m_up;
        private Vector3 m_right;

        private Matrix4 m_localToParentMatrix;
        private Matrix4 m_localToWorldMatrix;
        private Matrix4 m_worldToLocalMatrix;

        /// <summary>
        /// Bitmask flags that indicate if a related property's cache is invalidated.
        /// </summary>
        [Flags]
        private enum DirtyFlag
        {
            None            = 0,

            LocalPos        = (1 << 0),
            LocalRot        = (1 << 1),
            LocalScl        = (1 << 2),
            LocalScalars    = LocalPos | LocalRot | LocalScl,
            
            Forward         = (1 << 6),
            Upward          = (1 << 7),
            Right           = (1 << 8),
            Directions      = Forward | Upward | Right,

            LtPMat          = (1 << 20),

            LtWMat          = (1 << 26),
            WtLMat          = (1 << 27),
            World           = LtWMat | WtLMat,
            
            All             = LocalScalars | Directions | LtPMat | World,
        }

        /// <summary>
        /// Flags that indicate which properties caches are currently invalidated.
        /// </summary>
        private DirtyFlag m_dirtyFlags;

        /// <summary>
        /// The local space position of the transform.
        /// </summary>
        public Vector3 LocalPosition
        {
            get
            {
                RefreshLocalPosition();
                return m_localPosition;
            }
            set
            {
                if (m_localPosition != value)
                {
                    RefreshLocalRotation();
                    RefreshLocalScale();
                    ChildrenCacheLocal();
                    m_localPosition = value;
                    MarkClean(DirtyFlag.LocalPos);
                    MarkDirty(DirtyFlag.World | DirtyFlag.LtPMat);
                }
            }
        }
        
        /// <summary>
        /// The local space rotation of the transform.
        /// </summary>
        public Quaternion LocalRotation
        {
            get
            {
                RefreshLocalRotation();
                return m_localRotation;
            }
            set
            {
                if (m_localRotation != value)
                {
                    RefreshLocalPosition();
                    RefreshLocalScale();
                    ChildrenCacheLocal();
                    m_localRotation = value;
                    MarkClean(DirtyFlag.LocalRot);
                    MarkDirty(DirtyFlag.World | DirtyFlag.LtPMat | DirtyFlag.Directions);
                }
            }
        }

        /// <summary>
        /// The local space scale of the transform.
        /// </summary>
        public Vector3 LocalScale
        {
            get
            {
                RefreshLocalScale();
                return m_localScale;
            }
            set
            {
                if (m_localScale != value)
                {
                    RefreshLocalPosition();
                    RefreshLocalRotation();
                    ChildrenCacheLocal();
                    m_localScale = value;
                    MarkClean(DirtyFlag.LocalScl);
                    MarkDirty(DirtyFlag.World | DirtyFlag.LtPMat);
                }
            }
        }

        /// <summary>
        /// The matrix representing the local space orientation.
        /// </summary>
        private Matrix4 LocalToParentMatix
        {
            get
            {
                RefreshLocalToParent();
                return m_localToParentMatrix;
            }
        }

        /// <summary>
        /// The matrix representing the conversion from local space to world space.
        /// </summary>
        public Matrix4 LocalToWorldMatix
        {
            get
            {
                RefreshLocalToWorld();
                return m_localToWorldMatrix;
            }
            set
            {
                if (m_localToWorldMatrix != value)
                {
                    ChildrenCacheLocal();
                    m_localToWorldMatrix = value;
                    MarkClean(DirtyFlag.LtWMat);
                    MarkDirty(DirtyFlag.All);
                }
            }
        }

        /// <summary>
        /// The matrix representing the conversion from world space to local space.
        /// </summary>
        public Matrix4 WorldToLocalMatrix
        {
            get
            {
                RefreshWorldToLocal();
                return m_worldToLocalMatrix;
            }
            set
            {
                if (m_worldToLocalMatrix != value)
                {
                    ChildrenCacheLocal();
                    m_worldToLocalMatrix = value;
                    MarkClean(DirtyFlag.WtLMat);
                    MarkDirty(DirtyFlag.All);
                }
            }
        }

        /// <summary>
        /// The direction of the local negative unit Y vector, pointing in the transform's forward direction.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                if (IsAllDirty(DirtyFlag.Forward))
                {
                    m_forward = Vector3.TransformVector(-Vector3.UnitY, LocalToWorldMatix).Normalized();
                    MarkClean(DirtyFlag.Forward);
                }
                return m_forward;
            }
        }

        /// <summary>
        /// The direction of the local unit Z vector, pointing in the transform's upwards direction.
        /// </summary>
        public Vector3 Up
        {
            get
            {
                if (IsAllDirty(DirtyFlag.Upward))
                {
                    m_up = Vector3.TransformVector(Vector3.UnitZ, LocalToWorldMatix).Normalized();
                    MarkClean(DirtyFlag.Upward);
                }
                return m_up;
            }
        }

        /// <summary>
        /// The direction of the local negative unit X vector, pointing in the transform's right direction.
        /// </summary>
        public Vector3 Right
        {
            get
            {
                if (IsAllDirty(DirtyFlag.Right))
                {
                    m_right = Vector3.TransformVector(-Vector3.UnitX, LocalToWorldMatix).Normalized();
                    MarkClean(DirtyFlag.Right);
                }
                return m_right;
            }
        }

        private Transform m_parent;

        public delegate void ParentChangedHandler(Transform oldParent, Transform newParent);
        private ParentChangedHandler m_onParentChanged;

        /// <summary>
        /// The parent transform.
        /// </summary>
        public Transform Parent
        {
            get { return m_parent; }
            set { SetParent(value); }
        }

        /// <summary>
        /// The root ancestor transform.
        /// </summary>
        public Transform Root
        {
            get
            {
                Transform current = this;
                while (current.Parent != null)
                {
                    current = current.Parent;
                }
                return current;
            }
        }

        private List<Transform> m_children;

        /// <summary>
        /// The children of this transform.
        /// </summary>
        public IReadOnlyList<Transform> Children
        {
            get { return m_children; }
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity to attach this component to.</param>
        /// <param name="onParentChanged">Callback activated when </param>
        public Transform(Entity entity, ParentChangedHandler onParentChanged) : base(entity)
        {
            m_children = new List<Transform>();
            
            m_localPosition   = Vector3.Zero;
            m_localRotation   = Quaternion.Identity;
            m_localScale      = Vector3.One;

            m_forward   = -Vector3.UnitY;
            m_up        = Vector3.UnitZ;
            m_right     = -Vector3.UnitX;

            m_localToParentMatrix = Matrix4.Identity;
            m_localToWorldMatrix = Matrix4.Identity;
            m_worldToLocalMatrix = Matrix4.Identity;

            m_dirtyFlags = DirtyFlag.None;

            m_onParentChanged = onParentChanged;
        }

        /// <summary>
        /// Sets the parent of this transform.
        /// </summary>
        /// <param name="newParent">The transform to parent to. If null clears the parent.</param>
        /// <param name="worldPositionStays">If true the world orientation of theis transform will not change.</param>
        public void SetParent(Transform newParent, bool worldPositionStays = true)
        {
            if (m_parent != newParent)
            {
                Transform oldParent = m_parent;

                if (oldParent != null)
                {
                    oldParent.m_children.Remove(this);
                }
                if (newParent != null)
                {
                    newParent.m_children.Add(this);
                }

                // make sure the desired orientation is known before the parent change
                if (worldPositionStays)
                {
                    RefreshLocalToWorld();
                    MarkDirty(DirtyFlag.LocalScalars | DirtyFlag.LtPMat);
                }
                else
                {
                    RefreshLocalToParent();
                    MarkDirty(DirtyFlag.World | DirtyFlag.Directions);
                }

                m_parent = newParent;
                
                // notify the parent change
                if (m_onParentChanged != null)
                {
                    m_onParentChanged(oldParent, newParent);
                }
            }
        }

        /// <summary>
        /// Recursively iterates all childen of this transform,
        /// making sure that their local orientation is not dirty.
        /// </summary>
        private void ChildrenCacheLocal()
        {
            foreach (Transform child in m_children)
            {
                if (child.IsAnyDirty(DirtyFlag.LocalScalars))
                {
                    child.RefreshLocalToParent();
                }
                child.ChildrenCacheLocal();
            }
        }

        /// <summary>
        /// Updates the local position information.
        /// </summary>
        private void RefreshLocalPosition()
        {
            if (IsAllDirty(DirtyFlag.LocalPos))
            {
                m_localPosition = LocalToParentMatix.ExtractTranslation();
                MarkClean(DirtyFlag.LocalPos);
            }
        }

        /// <summary>
        /// Updates the local rotation information.
        /// </summary>
        private void RefreshLocalRotation()
        {
            if (IsAllDirty(DirtyFlag.LocalRot))
            {
                m_localRotation = LocalToParentMatix.ExtractRotation();
                MarkClean(DirtyFlag.LocalRot);
            }
        }

        /// <summary>
        /// Updates the local scale information.
        /// </summary>
        private void RefreshLocalScale()
        {
            if (IsAllDirty(DirtyFlag.LocalScl))
            {
                m_localScale = LocalToParentMatix.ExtractScale();
                MarkClean(DirtyFlag.LocalScl);
            }
        }

        /// <summary>
        /// Updates the parent to local matrix based on clean information.
        /// </summary>
        private void RefreshLocalToParent()
        {
            if (IsAllDirty(DirtyFlag.LtPMat))
            {
                if (IsAllClean(DirtyFlag.LocalScalars))
                {
                    Utils.CreateTransform(ref m_localPosition, ref m_localRotation, ref m_localScale, out m_localToParentMatrix);
                }
                else
                {
                    if (m_parent != null)
                    {
                        m_localToParentMatrix = LocalToWorldMatix * m_parent.WorldToLocalMatrix;
                    }
                    else
                    {
                        m_localToParentMatrix = LocalToWorldMatix;
                    }
                }
                MarkClean(DirtyFlag.LtPMat);
            }
        }

        /// <summary>
        /// Updates the local to world matrix based on clean information.
        /// </summary>
        private void RefreshLocalToWorld()
        {
            if (IsAllDirty(DirtyFlag.LtWMat))
            {
                if (IsAllClean(DirtyFlag.WtLMat))
                {
                    m_localToWorldMatrix = WorldToLocalMatrix.Inverted();
                }
                else if (m_parent != null)
                {
                    m_localToWorldMatrix = LocalToParentMatix * m_parent.LocalToWorldMatix;
                }
                else
                {
                    m_localToWorldMatrix = LocalToParentMatix;
                }
                MarkClean(DirtyFlag.LtWMat);
            }
        }

        /// <summary>
        /// Updates the world to local matrix based on clean information.
        /// </summary>
        private void RefreshWorldToLocal()
        {
            if (IsAllDirty(DirtyFlag.WtLMat))
            {
                m_worldToLocalMatrix = LocalToWorldMatix.Inverted();
                MarkClean(DirtyFlag.WtLMat);
            }
        }

        /// <summary>
        /// Ensures that the specified properties are updated when next accessed.
        /// </summary>
        /// <param name="flags">The properties to invalidate the caches of.</param>
        private void MarkDirty(DirtyFlag flags)
        {
            m_dirtyFlags |= flags;

            // check that the transform's orientation value can still be computed from any valid caches
            if (IsAllDirty(DirtyFlag.World | DirtyFlag.LtPMat) && IsAnyDirty(DirtyFlag.LocalScalars))
            {
                Logger.Error($"Transform on entity \"{Entity}\" has entered an unrecoverable dirty state: {m_dirtyFlags}");
            }

            if (IsAllDirty(DirtyFlag.World) || IsAnyDirty(DirtyFlag.Directions))
            {
                foreach (Transform child in m_children)
                {
                    child.MarkDirty(DirtyFlag.World | DirtyFlag.Directions);
                }
            }
        }

        /// <summary>
        /// Marks the flagged properties' caches as valid. 
        /// </summary>
        /// <param name="flags">The properties to make clean.</param>
        private void MarkClean(DirtyFlag flags)
        {
            m_dirtyFlags &= (~flags);
        }
        
        /// <summary>
        /// Checks if any flagged properties' caches are valid.
        /// </summary>
        /// <param name="flags">The properties check if clean.</param>
        /// <returns>True if any of the flagged properties are clean.</returns>
        private bool IsAnyClean(DirtyFlag flags)
        {
            return (m_dirtyFlags & flags) != flags;
        }

        /// <summary>
        /// Checks if all flagged properties' caches are valid.
        /// </summary>
        /// <param name="flags">The properties check if clean.</param>
        /// <returns>True if all of the flagged properties are clean.</returns>
        private bool IsAllClean(DirtyFlag flags)
        {
            return (m_dirtyFlags & flags) == 0;
        }

        /// <summary>
        /// Checks if any flagged properties' caches are invalid.
        /// </summary>
        /// <param name="flags">The properties check if dirty.</param>
        /// <returns>True if all of the properties are dirty.</returns>
        private bool IsAnyDirty(DirtyFlag flags)
        {
            return (m_dirtyFlags & flags) != 0;
        }

        /// <summary>
        /// Checks if any flagged properties' caches are invalid.
        /// </summary>
        /// <param name="flags">The properties check if dirty.</param>
        /// <returns>True if all of the properties are dirty.</returns>
        private bool IsAllDirty(DirtyFlag flags)
        {
            return (m_dirtyFlags & flags) == flags;
        }
    }
}
