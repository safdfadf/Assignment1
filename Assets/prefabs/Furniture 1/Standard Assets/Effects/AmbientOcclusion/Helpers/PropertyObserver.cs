using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    public partial class AmbientOcclusion : MonoBehaviour
    {
        // Observer class that detects changes on properties
        private struct PropertyObserver
        {
            // AO properties
            private bool _downsampling;
            private OcclusionSource _occlusionSource;
            private bool _ambientOnly;
            private bool _debug;

            // Camera properties
            private int _pixelWidth;
            private int _pixelHeight;

            // Check if it has to reset itself for property changes.
            public bool CheckNeedsReset(Settings setting, Camera camera)
            {
                return
                    _downsampling != setting.downsampling ||
                    _occlusionSource != setting.occlusionSource ||
                    _ambientOnly != setting.ambientOnly ||
                    _debug != setting.debug ||
                    _pixelWidth != camera.pixelWidth ||
                    _pixelHeight != camera.pixelHeight;
            }

            // Update the internal state.
            public void Update(Settings setting, Camera camera)
            {
                _downsampling = setting.downsampling;
                _occlusionSource = setting.occlusionSource;
                _ambientOnly = setting.ambientOnly;
                _debug = setting.debug;
                _pixelWidth = camera.pixelWidth;
                _pixelHeight = camera.pixelHeight;
            }
        }
    }
}