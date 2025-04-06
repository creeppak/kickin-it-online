using UnityEngine;

namespace Sources.Clean.Presentation.SupportScene
{
    public class DisabledBackgroundWorker : IBackgroundWorker
    {
        public void SetFadeOut()
        {
            Debug.LogError($"Ignoring {nameof(SetFadeOut)} call...");
        }

        public void ResetFadeOut()
        {
            Debug.LogError($"Ignoring {nameof(ResetFadeOut)} call...");
        }
    }
}