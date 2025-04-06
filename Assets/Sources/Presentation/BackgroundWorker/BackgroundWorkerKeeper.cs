using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Presentation.SupportScene
{
    public class BackgroundWorkerKeeper : IAsyncStartable, IDisposable
    {
        private readonly LifetimeScope _lifetimeScope;
        private readonly BackgroundWorkerProxy _backgroundWorkerProxy;
        private readonly IBackgroundWorkerConfig _config;
        
        private Scene _backgroundScene;

        public BackgroundWorkerKeeper(LifetimeScope lifetimeScope, BackgroundWorkerProxy backgroundWorkerProxy, IBackgroundWorkerConfig config)
        {
            _config = config;
            _backgroundWorkerProxy = backgroundWorkerProxy;
            _lifetimeScope = lifetimeScope;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            using (LifetimeScope.EnqueueParent(_lifetimeScope))
            {
                await SceneManager.LoadSceneAsync(_config.BackgroundSceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: cancellation);
            }
            
            _backgroundScene = SceneManager.GetSceneByName(_config.BackgroundSceneName);
            var backgroundScope = LifetimeScope.Find<BackgroundWorkerScope>(_backgroundScene);
            var worker = backgroundScope.Container.Resolve<IBackgroundWorker>();
            _backgroundWorkerProxy.SetWorker(worker);
        }

        public async void Dispose()
        {
            // var unloadOperation = SceneManager.UnloadSceneAsync(_backgroundScene);
            //
            // if (unloadOperation is null)
            // {
            //     Debug.Log("Tried unloading background scene, but it was likely the last scene loaded. Ignoring the unload request...");
            //     return;
            // }
            //
            // await unloadOperation.ToUniTask();
        }
    }
}