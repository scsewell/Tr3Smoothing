using System.Collections.Generic;
using System.Numerics;

namespace SoSmooth
{
    /// <summary>
    /// Represents a single segmentation line of a structure along a single slice.
    /// </summary>
    public class Contour
    {
        /// <summary>
        /// The name of the contour.
        /// </summary>
        public string Name { get { return m_name; } }
        private string m_name;

        /// <summary>
        /// The description of the contour.
        /// </summary>
        public string Description { get { return m_description; } }
        private string m_description;

        /// <summary>
        /// Selects how the contour is defined.
        /// </summary>
        public ContourType Type { get { return m_type; } }
        private ContourType m_type;

        /// <summary>
        /// Determines if the contour does joins the start and finish point.
        /// </summary>
        public bool IsOpen { get { return m_isOpen; } }
        private bool m_isOpen;

        private List<Connection> m_inboundConnections = new List<Connection>();
        private List<Connection> m_startConnections = new List<Connection>();
        private List<Connection> m_endConnections = new List<Connection>();

        private List<Vector3> m_points = new List<Vector3>();

        /// <summary>
        /// Connections from other contours.
        /// </summary>
        public IReadOnlyList<Connection> InboundConnections
        {
            get { return m_inboundConnections; }
        }

        /// <summary>
        /// Connection from the start point of this contour to another contour.
        /// </summary>
        public IReadOnlyList<Connection> StartConnections
        {
            get { return m_startConnections; }
        }

        /// <summary>
        /// Connection from the end point of this contour to another contour.
        /// </summary>
        public IReadOnlyList<Connection> EndConnections
        {
            get { return m_endConnections; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Contour(string name, string description, ContourType type, bool isOpen)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Logger.Error("Attemped to create a contour with an invalid name: " + name);
            }
            m_name = name;

            if (string.IsNullOrWhiteSpace(description))
            {
                Logger.Error("Attemped to create a contour with an invalid description: " + description);
            }
            m_description = description;

            m_type = type;
            m_isOpen = isOpen;
        }

        /// <summary>
        /// Deep copy constructor.
        /// </summary>
        /// <param name="contour">A contour to copy.</param>
        public Contour(Contour contour)
        {
            m_name          = contour.Name;
            m_description   = contour.Description;
            m_type          = contour.Type;
            m_isOpen        = contour.IsOpen;

            // copy all points
            m_points = new List<Vector3>(contour.m_points);
        }

        /// <summary>
        /// Adds a connection to or from another contour.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        public void AddConnection(Connection connection)
        {
            List<Connection> connections;

            if (connection.Source == this)
            {
                connections = (connection.SourcePoint == SourcePoint.Start) ? m_startConnections : m_endConnections;
            }
            else
            {
                connections = m_inboundConnections;
            }

            // ensure the connection is unique
            if (!connections.Contains(connection))
            {
                connections.Add(connection);
            }
        }

        /// <summary>
        /// Adds a point to this contour.
        /// </summary>
        /// <param name="point">The point to add.</param>
        public void AddPoint(Vector3 point)
        {
            m_points.Add(point);
        }

        /// <summary>
        /// Generates an informative description of this instance.
        /// </summary>
        public override string ToString()
        {
            return m_name;
        }
    }
}
