using UnityEngine;

namespace KickinIt.Presentation.BackgroundWorker
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