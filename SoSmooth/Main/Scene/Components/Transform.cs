using OpenTK;

namespace SoSmooth.Scene
{
    /// <summary>
    /// Stores spatial orientation and scenegraph heirarchy data.
    /// All entities must have a transform.
    /// </summary>
    public class Transform : Component
    {
        private Transform m_parent;

        /// <summary>
        /// The local space position of the transform.
        /// </summary>
        public Transform Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        private Vector3 m_position = Vector3.Zero;
        private Quaternion m_rotation = Quaternion.Identity;
        private Vector3 m_scale = Vector3.One;
        
        /// <summary>
        /// The local space position of the transform.
        /// </summary>
        public Vector3 LocalPosition
        {
            get { return m_position; }
            set { m_position = value; MarkMatriciesDirty(); }
        }

        /// <summary>
        /// The local space position of the transform.
        /// </summary>
        public Quaternion LocalRotation
        {
            get { return m_rotation; }
            set { m_rotation = value; MarkMatriciesDirty(); }
        }

        /// <summary>
        /// The local space position of the transform.
        /// </summary>
        public Vector3 LocalScale
        {
            get { return m_scale; }
            set { m_scale = value; MarkMatriciesDirty(); }
        }

        private Matrix4 m_localToWorldMatrix;
        private bool m_localToWorldDirty = true;

        /// <summary>
        /// The matrix representing the conversion from local space to world space.
        /// </summary>
        public Matrix4 LocalToWorldMatix
        {
            set
            {
                m_localToWorldMatrix = value;

                m_position = value.ExtractTranslation();
                m_rotation = value.ExtractRotation();
                m_scale = value.ExtractScale();
                
                m_localToWorldDirty = false;
                m_worldToLocalDirty = true;
            }
            get
            {
                if (m_localToWorldDirty)
                {
                    m_localToWorldMatrix =
                        Matrix4.CreateScale(m_scale) *
                        Matrix4.CreateFromQuaternion(m_rotation) *
                        Matrix4.CreateTranslation(m_position);

                    m_localToWorldDirty = false;
                }
                return m_localToWorldMatrix;
            }
        }

        private Matrix4 m_worldToLocalMatrix;
        private bool m_worldToLocalDirty = true;

        /// <summary>
        /// The matrix representing the conversion from world space to local space.
        /// </summary>
        public Matrix4 WorldToLocalMatrix
        {
            set
            {
                m_worldToLocalMatrix = value;
                LocalToWorldMatix = value.Inverted();
                m_worldToLocalDirty = false;
            }
            get
            {
                if (m_worldToLocalDirty)
                {
                    m_worldToLocalMatrix = LocalToWorldMatix.Inverted();
                    m_worldToLocalDirty = false;
                }
                return m_worldToLocalMatrix;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Transform(Entity entity) : base(entity)
        {
            MarkMatriciesDirty();
        }
        
        /// <summary>
        /// Ensures that the transformation matrices are updated when next accessed.
        /// </summary>
        private void MarkMatriciesDirty()
        {
            m_localToWorldDirty = true;
            m_worldToLocalDirty = true;
        }
    }
}
