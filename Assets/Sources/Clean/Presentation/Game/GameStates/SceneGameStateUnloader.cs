using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Sources.Clean.Presentation
{
    public class SceneGameStateUnloader : IGameStateUnloader
    {
        private readonly LifetimeScope _lifetimeScope;

        public SceneGameStateUnloader(LifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }
        
        public UniTask UnloadState()
        {
            if (!_lifetimeScope.gameObject)
            {
                Debug.LogError("The game state's scope was already unloaded. Ignoring unload request...");
                return UniTask.CompletedTask;
            }

            return SceneManager.UnloadSceneAsync(_lifetimeScope.gameObject.scene).ToUniTask();
        }
    }
}