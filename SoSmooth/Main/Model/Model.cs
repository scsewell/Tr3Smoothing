using System.Collections.Generic;
using System.Linq;

namespace SoSmooth
{
    /// <summary>
    /// Stores the segmentation data and information.
    /// </summary>
    public class Model
    {
        private Dictionary<string, Contour> m_nameToContour = new Dictionary<string, Contour>();
        private List<Contour> m_contours = new List<Contour>();
        private List<Connection> m_connections = new List<Connection>();

        /// <summary>
        /// The contours that represent the segmentation.
        /// </summary>
        public IReadOnlyList<Contour> Contours { get { return m_contours; } }

        /// <summary>
        /// The start and end connections from open contours to other contours
        /// </summary>
        public IReadOnlyList<Connection> Connections { get { return m_connections; } }

        /// <summary>
        /// Contructor.
        /// </summary>
        public Model() {}

        /// <summary>
        /// Deep copy contructor.
        /// </summary>
        /// <param name="model">A model to copy.</param>
        public Model(Model model)
        {
            Logger.Info("Cloning model");

            // Clone all contours
            foreach (Contour contour in model.Contours)
            {
                Contour clone = new Contour(contour);

                m_contours.Add(clone);
                m_nameToContour.Add(clone.Name, clone);
            }

            // Clone all connections
            foreach (Connection connection in model.Connections)
            {
                m_connections.Add(new Connection(connection));
            }

            Initialize();
        }

        /// <summary>
        /// Sets up the references between contours and connections.
        /// Must be called before use of the model.
        /// </summary>
        public void Initialize()
        {
            foreach (Connection c in m_connections)
            {
                c.SetReferences(m_nameToContour);
            }
            Logger.Info("Model initialized");
        }

        /// <summary>
        /// Adds a new contour to the model.
        /// </summary>
        /// <param name="contour">The contour to add.</param>
        public void AddContour(Contour contour)
        {
            if (m_nameToContour.ContainsKey(contour.Name))
            {
                Logger.Error("Cannot add multiple contours with the name name! Contours with duplicate names are ignored!");
            }
            else
            {
                m_contours.Add(contour);
                m_nameToContour.Add(contour.Name, contour);
            }
        }

        /// <summary>
        /// Adds a new connection to the model.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        public void AddConnection(Connection connection)
        {
            m_connections.Add(connection);
        }
    }
}
