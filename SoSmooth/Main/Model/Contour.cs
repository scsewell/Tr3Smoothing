using System.Collections.Generic;
using System.Linq;

namespace SoSmooth
{
    /// <summary>
    /// Represents a single segmentation contour of a structure along a single slice.
    /// </summary>
    public class Contour
    {
        private string m_name;
        public string Name { get { return m_name; } }

        private string m_description;
        public string Description { get { return m_description; } }

        private bool m_isOpen;
        public bool IsOpen { get { return m_isOpen; } }

        private ContourType m_type;
        public ContourType Type { get { return m_type; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Contour(
            string name,
            string description,
            bool isOpen,
            ContourType type
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
            m_description   = description;

            m_isOpen = isOpen;
            m_type = type;
        }

        /// <summary>
        /// Deep Copy Constructor.
        /// </summary>
        /// <param name="contour">A contour to copy.</param>
        public Contour(Contour contour)
        {
            m_name          = contour.Name;
            m_description   = contour.Description;
            m_isOpen        = contour.IsOpen;
            m_type          = contour.Type;
        }
    }
}
