using System.Collections.Generic;

namespace SoSmooth
{
    /// <summary>
    /// Stores the segmentation data and information.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// The contours that represent the segmentation (called "lines" in Fie)
        /// </summary>
        public IReadOnlyList<Contour> Contours { get { return m_contours; } }
        private List<Contour> m_contours;

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="contours">The set of contours that make up the model.</param>
        public Model(List<Contour> contours)
        {
            m_contours = contours;
        }

        /// <summary>
        /// Deep copy contructor.
        /// </summary>
        /// <param name="model">A model to copy.</param>
        public Model(Model model)
        {
            m_contours = new List<Contour>();

            foreach (Contour contour in model.Contours)
            {
                m_contours.Add(new Contour(contour));
            }
        }
    }
}
