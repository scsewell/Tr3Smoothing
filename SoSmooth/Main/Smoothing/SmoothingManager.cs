using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoSmooth
{
    /// <summary>
    /// Manages the settings and application of the smoothing algorithms.
    /// </summary>
    public class SmoothingManager : Singleton<SmoothingManager>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SmoothingManager()
        {
        }

        /// <summary>
        /// Smooths the selected meshes.
        /// </summary>
        public void SmoothSelected()
        {
            List<MeshInfo> selectedMeshes = MeshManager.Instance.SelectedMeshes;

            if (selectedMeshes.Count > 0 && MeanSmoother.Strength > 0 && MeanSmoother.Iterations > 0)
            {
                Logger.Info($"Smoothing {selectedMeshes.Count} meshes using mean filter");
                new SmoothOperation(selectedMeshes);
            }
        }
    }
}
