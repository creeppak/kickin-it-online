using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.BackgroundWorker
{
    public class BackgroundWorkerKeeper : IAsyncStartable
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
    }
}