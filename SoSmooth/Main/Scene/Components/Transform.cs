using System;
using System.Collections.Generic;
using OpenTK;

namespace SoSmooth.Scenes
{
    /// <summary>
    /// Stores spatial orientation and scenegraph heirarchy data.
    /// All entities must have a transform.
    /// </summary>
    public class Transform : Component
    {
        private Vector3 m_position;
        private bool m_positionDirty;

        private Quaternion m_rotation;
        private bool m_rotationDirty;

        private Vector3 m_scale;
        private bool m_scaleDirty;


        private Matrix4 m_parentToLocalMatrix;
        private bool m_parentToLocalDirty;

        private Matrix4 m_localToWorldMatrix;
        private bool m_localToWorldDirty;

        private Matrix4 m_worldToLocalMatrix;
        private bool m_worldToLocalDirty;


        /// <summary>
        /// The local space position of the transform.
        /// </summary>
        public Vector3 LocalPosition
        {
            get
            {
                RefreshLocalPosition();
                return m_position;
            }
            set
            {
                if (m_position != value)
                {
                    RefreshLocalRotation();
                    RefreshLocalScale();
                    m_position = value;
                    MarkDirty(true, true, false);
                }
                m_positionDirty = false;
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
                return m_rotation;
            }
            set
            {
                if (m_rotation != value)
                {
                    RefreshLocalPosition();
                    RefreshLocalScale();
                    m_rotation = value;
                    MarkDirty(true, true, false);
                }
                m_rotationDirty = false;
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
                return m_scale;
            }
            set
            {
                if (m_scale != value)
                {
                    RefreshLocalPosition();
                    RefreshLocalRotation();
                    m_scale = value;
                    MarkDirty(true, true, false);
                }
                m_scaleDirty = false;
            }
        }

        /// <summary>
        /// The matrix representing the local space orientation.
        /// </summary>
        private Matrix4 ParentToLocalMatix
        {
            get
            {
                RefreshParentToLocal();
                return m_parentToLocalMatrix;
            }
            set
            {
                if (m_parentToLocalMatrix != value)
                {
                    m_parentToLocalMatrix = value;
                    MarkDirty(true, true, true);
                }
                m_parentToLocalDirty = false;
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
                    MarkDirty(true, true, true);
                }
                m_localToWorldDirty = false;
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
                    MarkDirty(true, true, true);
                }
                m_worldToLocalDirty = false;
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
            
            m_position   = Vector3.Zero;
            m_rotation   = Quaternion.Identity;
            m_scale      = Vector3.One;

            MarkDirty(true, true, false);

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
                // we must know at least one world transformation matrix
                if (m_localToWorldDirty && m_worldToLocalDirty)
                {
                    RefreshWorldToLocal();
                }

                Transform oldParent = m_parent;

                if (oldParent != null)
                {
                    oldParent.m_children.Remove(this);
                }

                m_parent = newParent;

                if (newParent != null)
                {
                    newParent.m_children.Add(this);
                }

                // if not preserving the local space orientation, use the old world orientation
                // and the new parent's world orientation to get the new local orientation
                if (!worldPositionStays)
                {
                    RefreshParentToLocal();
                }
                MarkDirty(!worldPositionStays, worldPositionStays, true);

                // notify the parent change
                if (m_onParentChanged != null)
                {
                    m_onParentChanged(oldParent, newParent);
                }
            }
        }

        /// <summary>
        /// Ensures that the transformation matrices are updated when next accessed.
        /// </summary>
        /// <param name="worldMatrixDirty">The world matricies are dirtied.</param>
        /// <param name="localMatrixDirty">The local matrix is dirtied.</param>
        /// <param name="localScalarsDirty">The local space orentation scalars are dirtied.</param>
        private void MarkDirty(bool worldMatrixDirty, bool localMatrixDirty, bool localScalarsDirty)
        {
            if (localMatrixDirty)
            {
                m_parentToLocalDirty = true;
            }

            if (localScalarsDirty)
            {
                m_positionDirty = true;
                m_rotationDirty = true;
                m_scaleDirty = true;
            }

            if (worldMatrixDirty)
            {
                m_localToWorldDirty = true;
                m_worldToLocalDirty = true;

                // The world space transforms for children must also be updated.
                foreach (Transform child in m_children)
                {
                    // Ensure the local orientation is known by childern so they can find their
                    // new world orientation. Then mark their world orientation out of date, 
                    // since their parent has moved in world space.
                    child.RefreshParentToLocal();
                    child.MarkDirty(true, false, false);
                }
            }
        }

        /// <summary>
        /// Updates the local position information.
        /// </summary>
        private void RefreshLocalPosition()
        {
            if (m_positionDirty)
            {
                m_position = ParentToLocalMatix.Inverted().ExtractTranslation();
            }
            m_positionDirty = false;
        }

        /// <summary>
        /// Updates the local rotation information.
        /// </summary>
        private void RefreshLocalRotation()
        {
            if (m_rotationDirty)
            {
                m_rotation = ParentToLocalMatix.Inverted().ExtractRotation();
            }
            m_rotationDirty = false;
        }

        /// <summary>
        /// Updates the local scale information.
        /// </summary>
        private void RefreshLocalScale()
        {
            if (m_scaleDirty)
            {
                m_scale = ParentToLocalMatix.Inverted().ExtractScale();
            }
            m_scaleDirty = false;
        }

        /// <summary>
        /// Updates the parent to local matrix based on clean information.
        /// </summary>
        private void RefreshParentToLocal()
        {
            if (m_parentToLocalDirty)
            {
                if ((!m_worldToLocalDirty || !m_worldToLocalDirty))
                {
                    if (m_parent != null)
                    {
                        m_parentToLocalMatrix = m_parent.LocalToWorldMatix * WorldToLocalMatrix;
                    }
                    else
                    {
                        m_parentToLocalMatrix = WorldToLocalMatrix;
                    }
                }
                else
                {
                    Vector3 scale = LocalScale;
                    scale.X = 1 / scale.X;
                    scale.Y = 1 / scale.Y;
                    scale.Z = 1 / scale.Z;

                    m_parentToLocalMatrix =
                        Matrix4.CreateTranslation(-LocalPosition) *
                        Matrix4.CreateFromQuaternion(LocalRotation.Inverted()) *
                        Matrix4.CreateScale(scale);
                }
            }
            m_parentToLocalDirty = false;
        }

        /// <summary>
        /// Updates the local to world matrix based on clean information.
        /// </summary>
        private void RefreshLocalToWorld()
        {
            if (m_localToWorldDirty)
            {
                m_localToWorldMatrix = WorldToLocalMatrix.Inverted();
            }
            m_localToWorldDirty = false;
        }

        /// <summary>
        /// Updates the world to local matrix based on clean information.
        /// </summary>
        private void RefreshWorldToLocal()
        {
            if (m_worldToLocalDirty)
            {
                if (!m_localToWorldDirty)
                {
                    m_worldToLocalMatrix = LocalToWorldMatix.Inverted();
                }
                else if (m_parent != null)
                {
                    m_worldToLocalMatrix = m_parent.WorldToLocalMatrix * ParentToLocalMatix;
                }
                else
                {
                    m_worldToLocalMatrix = ParentToLocalMatix;
                }
            }
            m_worldToLocalDirty = false;
        }
    }
}
