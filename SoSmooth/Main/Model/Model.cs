using System.Collections.Generic;

namespace SoSmooth
{
    /// <summary>
    /// Stores the segmentation data and information.
    /// </summary>
    public class Model
    {
        private List<Contour> m_contours;
        public IReadOnlyList<Contour> Contours { get { return m_contours; } }

        public Model(List<Contour> contours)
        {
            m_contours = contours;
        }
    }
}
