using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;

namespace SoSmooth
{
    /// <summary>
    /// Contains the smoothing UI.
    /// </summary>
    public class SmoothingContent : VBox
    {
        private HScale m_iterationCount;
        private HScale m_strength;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public SmoothingContent()
        {
            HeightRequest = 200;

            Label title = new Label();
            title.Markup = "<b>Smoothing</b>";
            PackStart(title, false, false, 5);

            m_iterationCount = new HScale(1, MeanSmoother.MAX_ITERATIONS, 1);
            m_iterationCount.Value = SmoothingManager.Instance.meanSmoother.Iterations;
            m_iterationCount.CanFocus = false;
            m_iterationCount.ValueChanged += OnIterationsChanged;
            CreateField(80, m_iterationCount, "Iterations", "The number of iterations performed by the smoothing algorithm.");

            m_strength = new HScale(0, 1, 0.01);
            m_strength.Value = SmoothingManager.Instance.meanSmoother.Strength;
            m_strength.CanFocus = false;
            m_strength.ValueChanged += OnStrengthChanged;
            CreateField(80, m_strength, "Strength", "The strength of the smoothing effect.");

            Button smoothButton = new Button();
            smoothButton.Label = "Smooth Selected";
            smoothButton.CanFocus = false;
            smoothButton.Clicked += OnSmoothButtonClick;
            PackStart(smoothButton, false, false, 5);
        }

        /// <summary>
        /// Called when the number of smoothing iterations has been changed.
        /// </summary>
        private void OnIterationsChanged(object sender, EventArgs e)
        {
            SmoothingManager.Instance.meanSmoother.Iterations = (int)Math.Round(m_iterationCount.Value);
        }

        /// <summary>
        /// Called when the smoothing strength has changed.
        /// </summary>
        private void OnStrengthChanged(object sender, EventArgs e)
        {
            SmoothingManager.Instance.meanSmoother.Strength = (float)m_strength.Value;
        }
        
        /// <summary>
        /// Called when the smooth button is pressed.
        /// </summary>
        private void OnSmoothButtonClick(object sender, EventArgs e)
        {
            SmoothingManager.Instance.SmoothSelected();
        }

        /// <summary>
        /// Creates a field with a title label and value widget.
        /// </summary>
        /// <param name="alignment">The pixel width of the title labels.</param>
        /// <param name="value">The widget that is shown as the value.</param>
        /// <param name="title">The title of the field.</param>
        /// <param name="tooltip">The tooltip displayed for the field.</param>
        private void CreateField(int alignment, Widget value, string title, string tooltip = null)
        {
            HBox box = new HBox();
            if (tooltip != null)
            {
                box.TooltipText = tooltip;
            }

            Label titleLabel = new Label(title + ":");
            titleLabel.WidthRequest = alignment;
            titleLabel.Xalign = 0;

            box.PackStart(titleLabel, false, false, 5);
            box.PackStart(value, true, true, 5);
            PackStart(box, false, false, 1);
        }
    }
}
