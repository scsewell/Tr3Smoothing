using System.Collections.Generic;
using System.Linq;

namespace SoSmooth
{
    /// <summary>
    /// Represents a single segmentation contour of a structure along a single slice.
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

        /// <summary>
        /// Connections from other contours.
        /// </summary>
        public IReadOnlyList<Connection> InboundConnections { get { return m_inboundConnections; } }
        private List<Connection> m_inboundConnections;

        /// <summary>
        /// Connection from the start point of this contour to another contour.
        /// </summary>
        public Connection StartConnection { get { return m_startConnection; } }
        private Connection m_startConnection;

        /// <summary>
        /// Connection from the end point of this contour to another contour.
        /// </summary>
        public Connection EndConnection { get { return m_endConnection; } }
        private Connection m_endConnection;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Contour(
            string name,
            string description,
            ContourType type,
            bool isOpen
            )
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
        }

        /// <summary>
        /// Adds a connection to or from another contour.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        public void AddConnection(Connection connection)
        {
            if (connection.Source == this)
            {
                if (connection.SourcePoint == SourcePoint.Start)
                {
                    m_startConnection = connection;
                }
                else
                {
                    m_endConnection = connection;
                }
            }
            else if (!m_inboundConnections.Contains(connection))
            {
                m_inboundConnections.Add(connection);
            }
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
