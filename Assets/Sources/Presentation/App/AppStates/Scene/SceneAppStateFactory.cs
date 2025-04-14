using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class SceneAppStateFactory : IAppStateFactory, IHotLoadAppStateProvider
    {
        private readonly IAppStateSceneProvider _sceneProvider;
        private readonly LifetimeScope _currentScope;

        public SceneAppStateFactory(IAppStateSceneProvider sceneProvider, LifetimeScope currentScope)
        {
            _currentScope = currentScope;
            _sceneProvider = sceneProvider;
        }
        
        public async Task<AppState> CreateState<TArgs>(AppStateId stateId, TArgs args)
        {
            var sceneName = _sceneProvider.GetSceneName(stateId);

            if (sceneName is null) return null;

            using (LifetimeScope.EnqueueParent(_currentScope))
            using (LifetimeScope.Enqueue(RegisterArgs))
            {
                var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                if (sceneLoad is null)
                {
                    throw new InvalidOperationException("Couldn't load game state scene.");
                }
            
                await sceneLoad; 

                var scene = SceneManager.GetSceneByName(sceneName);
                var gameStateScope = LifetimeScope.Find<AppStateSceneScope>(scene);
                if (!gameStateScope.autoRun)
                {
                    gameStateScope.Build();
                }
                var gameState = gameStateScope.Container.Resolve<AppState>();

                return gameState;
            }
            
            void RegisterArgs(IContainerBuilder builder)
            {
                if (args is null) return;

                builder.RegisterInstance(args);
            }
        }

        public bool TryRegisterHotLoadState(out AppState appState)
        {
            appState = null;
            
            var scene = SceneManager.GetActiveScene();
            var gameStateScope = (AppStateSceneScope) LifetimeScope.Find<AppStateSceneScope>(scene);
            
            if (!gameStateScope) { return false; }

            if (gameStateScope.RequireArguments)
            {
                Debug.Log("Can't boot from state that requires arguments.");
                return false;
            }
            
            using (LifetimeScope.EnqueueParent(_currentScope))
            {
                if (!gameStateScope.autoRun)
                {
                    gameStateScope.Build();
                }
                
                appState = gameStateScope.Container.Resolve<AppState>();
            }

            return true;
        }
    }
}