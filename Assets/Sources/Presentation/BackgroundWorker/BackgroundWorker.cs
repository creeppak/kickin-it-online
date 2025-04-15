using UnityEngine;
using UnityEngine.SceneManagement;

namespace KickinIt.Presentation.BackgroundWorker
{
    public class BackgroundWorker : MonoBehaviour, IBackgroundWorker
    {
        public void SetFadeOut()
        {
            throw new System.NotImplementedException(); // delegate to service, increment fade counter
        }

        public void ResetFadeOut()
        {
            throw new System.NotImplementedException();
        }

        public void AddGameObject(GameObject newBackgroundGameObject) // todo: remove?
        {
            SceneManager.MoveGameObjectToScene(newBackgroundGameObject, gameObject.scene);
        }
    }
}