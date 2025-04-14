using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Game.GameStates
{
    public class NetworkedAppStateFactory : IAppStateFactory
    {
        private readonly LifetimeScope _currentScope;
        private readonly IAppStateSceneProvider _sceneProvider;

        public NetworkedAppStateFactory(IAppStateSceneProvider sceneProvider, LifetimeScope currentScope)
        {
            _currentScope = currentScope;
            _sceneProvider = sceneProvider;
        }
        
        public async Task<AppState> CreateState<TArgs>(AppStateId stateId, TArgs args)
        {
            if (args is not NetworkedAppStateArgs networkedGameStateArgs) { return null; }
            
            var sceneName = _sceneProvider.GetSceneName(stateId);

            if (sceneName is null) { return null; }

            using (LifetimeScope.EnqueueParent(_currentScope))
            using (LifetimeScope.Enqueue(RegisterArgs))
            {
                if (networkedGameStateArgs.networkRunner.IsServer)
                {
                    await LoadSceneAsServer(sceneName, networkedGameStateArgs.networkRunner);
                }
                else
                {
                    await LoadSceneAsClient(sceneName, networkedGameStateArgs.networkRunner);
                }
            }
            
            void RegisterArgs(IContainerBuilder builder)
            {
                if (args is null) {return;}

                builder.RegisterInstance(args);
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            var gameStateScope = LifetimeScope.Find<AppStateSceneScope>(scene);
            var gameState = gameStateScope.Container.Resolve<AppState>();

            return gameState;
        }

        private static async Task LoadSceneAsClient(string sceneName, NetworkRunner networkRunner)
        {
            // network objects are already replicated, let's load the scene now
            var sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    
            await sceneLoadOperation.ToUniTask();

            // scene loaded, let's move network objects to the loaded scene
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            var networkObjects = networkRunner.GetAllNetworkObjects();
            foreach (var networkObject in networkObjects)
            {
                SceneManager.MoveGameObjectToScene(networkObject.gameObject, loadedScene);
            }
        }

        private static async Task LoadSceneAsServer(string sceneName, NetworkRunner networkRunner)
        {
            // command network runner to load scene
            var sceneLoadOperation = networkRunner.LoadScene(sceneName,
                LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);

            if (!sceneLoadOperation.IsValid)
            {
                throw new InvalidOperationException("Couldn't load game state scene.");
            }

            await sceneLoadOperation;
        }
    }
}