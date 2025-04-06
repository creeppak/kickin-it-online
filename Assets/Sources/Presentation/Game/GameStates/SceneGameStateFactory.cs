using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class SceneGameStateFactory : IGameStateFactory
    {
        private readonly IGameStateSceneProvider _sceneProvider;
        private readonly LifetimeScope _currentScope;

        public SceneGameStateFactory(IGameStateSceneProvider sceneProvider, LifetimeScope currentScope)
        {
            _currentScope = currentScope;
            _sceneProvider = sceneProvider;
        }
        
        public async Task<GameState> CreateState<TArgs>(GameStateId stateId, TArgs args)
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
            }
            
            void RegisterArgs(IContainerBuilder builder)
            {
                if (args is null) return;

                builder.RegisterInstance(args);
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            var gameStateScope = LifetimeScope.Find<GameStateSceneScope>(scene);
            var gameState = gameStateScope.Container.Resolve<GameState>();

            return gameState;
        }
    }
}