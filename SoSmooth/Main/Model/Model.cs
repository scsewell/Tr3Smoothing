using System.Collections.Generic;
using System.IO;

namespace SoSmooth
{
    /// <summary>
    /// Stores the segmentation data and information.
    /// </summary>
    public class Model
    {
        private FileInfo m_file;

        private List<Slice> m_slices = new List<Slice>();
        private Dictionary<string, Contour> m_nameToContour = new Dictionary<string, Contour>();
        private List<Contour> m_contours = new List<Contour>();
        private List<Connection> m_connections = new List<Connection>();

        private bool m_initialized = false;

        /// <summary>
        /// The image slices.
        /// </summary>
        public IReadOnlyList<Slice> Slices { get { return m_slices; } }

        /// <summary>
        /// The contours that represent the segmentation.
        /// </summary>
        public IReadOnlyList<Contour> Contours { get { return m_contours; } }

        /// <summary>
        /// The start and end connections from open contours to other contours.
        /// </summary>
        public IReadOnlyList<Connection> Connections { get { return m_connections; } }

        /// <summary>
        /// The tr3 file represented by this model.
        /// </summary>
        public FileInfo File { get { return m_file; } }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="file">The tr3 file represented by this model.</param>
        public Model(FileInfo file)
        {
            m_file = file;
        }

        /// <summary>
        /// Deep copy contructor.
        /// </summary>
        /// <param name="source">A model to copy.</param>
        public Model(Model source)
        {
            Logger.Info("Cloning model");

            m_file = source.File;

            // Copy slice references
            foreach (Slice slice in source.Slices)
            {
                m_slices.Add(slice);
            }

            // Clone all contours
            foreach (Contour contour in source.Contours)
            {
                Contour clone = new Contour(contour);

                m_contours.Add(clone);
                m_nameToContour.Add(clone.Name, clone);
            }

            // Clone all connections
            foreach (Connection connection in source.Connections)
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
            if (m_initialized)
            {
                Logger.Warning("Model has already been initialized!");
            }
            else
            {
                foreach (Connection c in m_connections)
                {
                    c.SetReferences(m_nameToContour);
                }
                m_initialized = true;
                Logger.Info("Model initialized");
            }
        }

        /// <summary>
        /// Adds a new slice to the model.
        /// </summary>
        /// <param name="slice">The slice to add.</param>
        public void AddSlice(Slice slice)
        {
            m_slices.Add(slice);
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

        /// <summary>
        /// Gets a contour by name.
        /// </summary>
        /// <param name="name">The name of the contour.</param>
        public Contour GetContour(string name)
        {
            Contour contour;
            if (!m_nameToContour.TryGetValue(name, out contour))
            {
                Logger.Error("Could not find contour with name: " + name);
            }
            return contour;
        }
    }
}
