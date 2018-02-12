using System;
using System.Collections.Generic;
using OpenTK;

namespace SoSmooth.Scenes
{

    /// <summary>
    /// Stores spatial orientation and scenegraph heirarchy data. All entities must
    /// have a transform.
    /// </summary>
    public class Transform : Component
    {
        private Vector3 m_localPosition;
        private Quaternion m_localRotation;
        private Vector3 m_localScale;
        
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

            LtPMat          = (1 << 3),
            Local           = LocalScalars | LtPMat,

            LtWMat          = (1 << 4),
            WtLMat          = (1 << 5),
            World           = LtWMat | WtLMat,
            
            All             = Local | World,
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
                    m_localPosition = value;
                    MarkDirty(DirtyFlag.World | DirtyFlag.LtPMat);
                }
                MarkClean(DirtyFlag.LocalPos);
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
                    m_localRotation = value;
                    MarkDirty(DirtyFlag.World | DirtyFlag.LtPMat);
                }
                MarkClean(DirtyFlag.LocalRot);
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
                    m_localScale = value;
                    MarkDirty(DirtyFlag.World | DirtyFlag.LtPMat);
                }
                MarkClean(DirtyFlag.LocalScl);
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
                    m_localToWorldMatrix = value;
                    MarkDirty(DirtyFlag.All);
                }
                MarkClean(DirtyFlag.LtWMat);
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
                    m_worldToLocalMatrix = value;
                    MarkDirty(DirtyFlag.All);
                }
                MarkClean(DirtyFlag.WtLMat);
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
                    MarkDirty(DirtyFlag.Local);
                }
                else
                {
                    RefreshLocalToParent();
                    MarkDirty(DirtyFlag.World);
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
        /// Updates the local position information.
        /// </summary>
        private void RefreshLocalPosition()
        {
            if (IsDirty(DirtyFlag.LocalPos))
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
            if (IsDirty(DirtyFlag.LocalRot))
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
            if (IsDirty(DirtyFlag.LocalScl))
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
            if (IsDirty(DirtyFlag.LtPMat))
            {
                if (IsClean(DirtyFlag.LocalScalars))
                {
                    CreateTransform(ref m_localPosition, ref m_localRotation, ref m_localScale, out m_localToParentMatrix);
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
            if (IsDirty(DirtyFlag.LtWMat))
            {
                if (IsClean(DirtyFlag.WtLMat))
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
            if (IsDirty(DirtyFlag.WtLMat))
            {
                m_worldToLocalMatrix = LocalToWorldMatix.Inverted();
                MarkClean(DirtyFlag.WtLMat);
            }
        }

        /// <summary>
        /// Checks if all given properties' caches are valid.
        /// </summary>
        /// <param name="flags">The properties check if clean.</param>
        /// <returns>True if all of the properties are clean.</returns>
        private bool IsClean(DirtyFlag flags)
        {
            return (m_dirtyFlags & flags) == 0;
        }

        /// <summary>
        /// Marks the given properties' caches as valid. 
        /// </summary>
        /// <param name="flags"></param>
        private void MarkClean(DirtyFlag flags)
        {
            m_dirtyFlags &= (~flags);
        }

        /// <summary>
        /// Checks if any given properties' caches are invalid.
        /// </summary>
        /// <param name="flags">The properties check if dirty.</param>
        /// <returns>True if any of the properties are dirty.</returns>
        private bool IsDirty(DirtyFlag flags)
        {
            return (m_dirtyFlags & flags) != 0;
        }
        
        /// <summary>
        /// Ensures that the specified properties are updated when next accessed.
        /// </summary>
        /// <param name="flags">The properties to invalidate the caches of.</param>
        private void MarkDirty(DirtyFlag flags)
        {
            m_dirtyFlags |= flags;

            // If the wold position has changed any children are also affected.
            // Ensure they known their local orientation so they can find their
            // new world orientation later before invalidating their cached world
            // orientation.
            if (IsDirty(DirtyFlag.World))
            {
                foreach (Transform child in m_children)
                {
                    child.RefreshLocalToParent();
                    child.MarkDirty(DirtyFlag.World);
                }
            }
        }

        /// <summary>
        /// Optimally a transformation matrix.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="result">A matrix instance.</param>
        public static void CreateTransform(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, out Matrix4 result)
        {
            result = Matrix4.Identity;
            
            Vector3 axis;
            float angle;
            rotation.ToAxisAngle(out axis, out angle);
            axis.Normalize();

            // calculate angles
            float cos = (float)Math.Cos(-angle);
            float sin = (float)Math.Sin(-angle);
            float t = 1.0f - cos;

            // do the conversion math once
            float tXX = t * axis.X * axis.X;
            float tXY = t * axis.X * axis.Y;
            float tXZ = t * axis.X * axis.Z;
            float tYY = t * axis.Y * axis.Y;
            float tYZ = t * axis.Y * axis.Z;
            float tZZ = t * axis.Z * axis.Z;

            float sinX = sin * axis.X;
            float sinY = sin * axis.Y;
            float sinZ = sin * axis.Z;

            result.Row0.X = scale.X * (tXX + cos);
            result.Row0.Y = scale.X * (tXY - sinZ);
            result.Row0.Z = scale.X * (tXZ + sinY);
            result.Row1.X = scale.Y * (tXY + sinZ);
            result.Row1.Y = scale.Y * (tYY + cos);
            result.Row1.Z = scale.Y * (tYZ - sinX);
            result.Row2.X = scale.Z * (tXZ - sinY);
            result.Row2.Y = scale.Z * (tYZ + sinX);
            result.Row2.Z = scale.Z * (tZZ + cos);

            // set the position
            result.Row3.X = position.X;
            result.Row3.Y = position.Y;
            result.Row3.Z = position.Z;
        }
    }
}
