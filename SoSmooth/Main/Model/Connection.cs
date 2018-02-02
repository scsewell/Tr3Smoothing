using System;

namespace SoSmooth
{
    /// <summary>
    /// Represents a connection from an endpoint of an open loop contour to another contour. 
    /// </summary>
    public class Connection : IEquatable<Connection>
    {
        /// <summary>
        /// The contour that connects to the target.
        /// </summary>
        public Contour Source { get { return m_source; } }
        private Contour m_source;

        /// <summary>
        /// Which point on the source contour to to start the connection from.
        /// </summary>
        public SourcePoint SourcePoint { get { return m_sourcePoint; } }
        private SourcePoint m_sourcePoint;
        
        /// <summary>
        /// The contour that is connected to.
        /// </summary>
        public Contour Target { get { return m_target; } }
        private Contour m_target;

        /// <summary>
        /// Which point on the target contour to connect to.
        /// </summary>
        public TargetPoint TargetPoint { get { return m_targetPoint; } }
        private TargetPoint m_targetPoint;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Connection(Contour source, SourcePoint sourcePoint, Contour target, TargetPoint targetPoint)
        {
            m_source = source;
            m_sourcePoint = sourcePoint;

            m_target = target;
            m_targetPoint = targetPoint;

            if (source == target)
            {
                Logger.Error("Source and target contour for a connection are the same: " + ToString());
            }

            // add a reference to this connection to each contour
            m_source.AddConnection(this);
            m_target.AddConnection(this);
        }

        /// <summary>
        /// Deep copy constructor.
        /// </summary>
        public Connection(Connection connection)
        {
            throw new NotImplementedException("Connection Deep Copy");
        }

        /// <summary>
        /// Compares this instance to another by value.
        /// </summary>
        /// <param name="other">The instance to compare with.</param>
        /// <returns>True if idendical by value.</returns>
        public bool Equals(Connection other)
        {
            if (other == null)
            {
                return false;
            }
            return 
                m_source == other.Source &&
                m_target == other.Target &&
                m_sourcePoint == other.SourcePoint &&
                m_targetPoint == other.TargetPoint;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return Equals(obj as Connection);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Generates an informative description of this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Connection from {0} {1} point to {2} {3} point", m_source, m_sourcePoint, m_target, m_targetPoint);
        }
    }
}
