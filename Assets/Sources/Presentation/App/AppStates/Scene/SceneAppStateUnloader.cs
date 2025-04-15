using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace KickinIt.Presentation.Game.GameStates
{
    public class SceneAppStateUnloader : IAppStateUnloader
    {
        private readonly LifetimeScope _lifetimeScope;
        private readonly AsyncDisposingSystem _asyncDisposingSystem;

        public SceneAppStateUnloader(LifetimeScope lifetimeScope, AsyncDisposingSystem asyncDisposingSystem)
        {
            _asyncDisposingSystem = asyncDisposingSystem;
            _lifetimeScope = lifetimeScope;
        }
        
        public async UniTask UnloadState()
        {
            if (!_lifetimeScope.gameObject)
            {
                Debug.LogError("The game state's scope was already unloaded. Ignoring unload request...");
                return;
            } 

            await _asyncDisposingSystem.DisposeAll();
            await SceneManager.UnloadSceneAsync(_lifetimeScope.gameObject.scene).ToUniTask();
        }
    }
}