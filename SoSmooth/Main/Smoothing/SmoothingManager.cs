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
        public readonly MeanSmoother meanSmoother = new MeanSmoother();

        /// <summary>
        /// The current smoothing algorithm.
        /// </summary>
        public ISmoother currentSmoother;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SmoothingManager()
        {
            currentSmoother = meanSmoother;
        }

        /// <summary>
        /// Smooths the selected meshes.
        /// </summary>
        public void SmoothSelected()
        {
            List<MeshInfo> selectedMeshes = MeshManager.Instance.SelectedMeshes;

            if (selectedMeshes.Count > 0 && !currentSmoother.WillNoOp())
            {
                Logger.Info($"Smoothing {selectedMeshes.Count} meshes using " + currentSmoother);

                new SmoothOperation(meanSmoother, selectedMeshes);
            }
        }
    }
}
