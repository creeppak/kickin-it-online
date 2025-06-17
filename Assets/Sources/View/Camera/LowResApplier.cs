using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KickinIt.Simulation.Camera
{
    public class LowResApplier : MonoBehaviour
    {
        public float targetLowResHeight = 135;
        
        private UniversalRenderPipelineAsset _urp;

        private void Awake()
        {
            _urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            UpdateResolution();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateResolution();
        }
#endif

        private void UpdateResolution()
        {
            if (!_urp)
            {
                Debug.LogError("No URP settings found.");
                enabled = false;
                return;
            }

            var targetScale = targetLowResHeight / Screen.height;
            var currentScale = _urp.renderScale;

            if (Mathf.Abs(targetScale - currentScale) < 0.001f)
            {
                return;
            }
            
            _urp.renderScale = targetScale;
        }
    }
}