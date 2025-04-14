using UnityEngine;

namespace KickinIt.Presentation.BackgroundWorker
{
    public interface IBackgroundWorker
    {
        public void SetFadeOut();
        public void ResetFadeOut();
        public void AddGameObject(GameObject gameObject);
    }
}