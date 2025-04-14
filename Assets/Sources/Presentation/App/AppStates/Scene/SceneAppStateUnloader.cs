using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace KickinIt.Presentation.Game.GameStates
{
    public class SceneAppStateUnloader : IAppStateUnloader
    {
        private readonly LifetimeScope _lifetimeScope;

        public SceneAppStateUnloader(LifetimeScope lifetimeScope)
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