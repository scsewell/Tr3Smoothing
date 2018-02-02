using System.Collections.Generic;

namespace SoSmooth
{
    /// <summary>
    /// Represents a connection from an endpoint of an open loop contour to another contour. 
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// The name of the contour that connects to the target.
        /// </summary>
        public string SourceName { get { return m_sourceName; } }
        private string m_sourceName;

        /// <summary>
        /// The name of the contour that is connected to.
        /// </summary>
        public string TargetName { get { return m_targetName; } }
        private string m_targetName;

        /// <summary>
        /// Which point on the source contour to to start the connection from.
        /// </summary>
        public SourcePoint SourcePoint { get { return m_sourcePoint; } }
        private SourcePoint m_sourcePoint;
        
        /// <summary>
        /// Which point on the target contour to connect to.
        /// </summary>
        public TargetPoint TargetPoint { get { return m_targetPoint; } }
        private TargetPoint m_targetPoint;

        /// <summary>
        /// 
        /// </summary>
        public int MinZ { get { return m_minZ; } }
        private int m_minZ;

        /// <summary>
        /// 
        /// </summary>
        public int MaxZ { get { return m_maxZ; } }
        private int m_maxZ;

        /// <summary>
        /// The contour that connects to the target.
        /// </summary>
        public Contour Source { get { return m_source; } }
        private Contour m_source;

        /// <summary>
        /// The contour that is connected to.
        /// </summary>
        public Contour Target { get { return m_target; } }
        private Contour m_target;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Connection(
            string sourceName,
            string targetName,
            SourcePoint sourcePoint,
            TargetPoint targetPoint,
            int minZ,
            int maxZ)
        {
            m_sourceName = sourceName;
            m_targetName = targetName;

            m_sourcePoint = sourcePoint;
            m_targetPoint = targetPoint;

            if (minZ > maxZ)
            {
                Logger.Error("MinZ is greater than MaxZ for connection: " + ToString());
            }

            m_minZ = minZ;
            m_maxZ = maxZ;
        }

        /// <summary>
        /// Deep copy constructor.
        /// </summary>
        public Connection(Connection connection)
        {
            m_sourceName = connection.SourceName;
            m_targetName = connection.TargetName;

            m_sourcePoint = connection.SourcePoint;
            m_targetPoint = connection.TargetPoint;

            m_minZ = connection.MinZ;
            m_maxZ = connection.MaxZ;
        }

        /// <summary>
        /// Update the contour fields with correct references and adds a
        /// reference to this connection to both contours.
        /// </summary>
        /// <param name="nameToContour">A mapping from contour names to contour instances.</param>
        public void SetReferences(Dictionary<string, Contour> nameToContour)
        {
            if (!nameToContour.TryGetValue(m_sourceName, out m_source))
            {
                Logger.Error("No contour with name: " + m_sourceName);
            }

            if (!nameToContour.TryGetValue(m_targetName, out m_target))
            {
                Logger.Error("No contour with name: " + m_targetName);
            }

            // add a reference to this connection to each contour
            m_source.AddConnection(this);
            m_target.AddConnection(this);
        }

        /// <summary>
        /// Generates an informative description of this instance.
        /// </summary>
        public override string ToString()
        {
            string range = (MinZ == int.MinValue ? "(" : "[" + MinZ) + ',' + (MaxZ == int.MaxValue ? ")" : MaxZ + "]");

            return string.Format(
                "{0} {1} point to {2} {3} point over slices {4}", 
                m_sourceName, m_sourcePoint, m_targetName, m_targetPoint, range
                );
        }
    }
}
