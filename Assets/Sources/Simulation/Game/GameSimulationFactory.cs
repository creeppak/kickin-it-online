using System;
using Cysharp.Threading.Tasks;
using Fusion;
using KickinIt.Simulation.Network;
using KickinIt.Simulation.Player;
using R3;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Game
{
    internal class GameSimulationFactory : IGameSimulationFactory
    {
        private readonly LifetimeScope _scope;
        private readonly INetworkRunnerContainer _networkRunnerContainer;

        public GameSimulationFactory(LifetimeScope scope, INetworkRunnerContainer networkRunnerContainer)
        {
            _networkRunnerContainer = networkRunnerContainer;
            _scope = scope;
        }

        public async UniTask<IGameSimulation> Create(SimulationArgs args)
        {
            const string simulationSceneName = "Game Simulation"; // using single scene for now
            
            var networkRunner = _networkRunnerContainer.InitializeNew();
            networkRunner.ProvideInput = true; // only host and client modes are supported for now, both provide input
            var sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            var sceneRef = sceneManager.GetSceneRef(simulationSceneName);
            GameSimulationScope simulationScope = null;
            
            if (args.host) // host
            {
                var sceneInfo = new NetworkSceneInfo();
                sceneInfo.AddSceneRef(sceneRef, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D, true);
                
                var result = await networkRunner.StartGame(new StartGameArgs
                {
                    GameMode = GameMode.Host,
                    SessionName = args.sessionCode,
                    Scene = sceneInfo,
                    SceneManager = sceneManager
                });

                if (!result.Ok)
                {
                    throw new Exception(
                        $"Game network component initialization failed. Shutdown reason: {result.ShutdownReason}. Custom error data: {result.ErrorMessage}. Stacktrace:\n{result.StackTrace}.");
                }
                
                await UniTask.WaitUntil(() => SceneManager.GetSceneByBuildIndex(sceneRef.AsIndex).isLoaded); // networkRunner.StartGame returns before the scene is actually loaded
                
                var loadedScene = SceneManager.GetSceneByBuildIndex(sceneRef.AsIndex);
                simulationScope = RegisterSimulationScope(loadedScene);
            }
            else // client
            {
                // we have to hack things a bit, because we don't have control over the replication process of Photon Fusion
                var subscription = ReplicationEvent<GameReplication>.ReplicatedOnClient
                    .Take(1)
                    .Subscribe(networkComponent =>
                    {
                        var scene = networkComponent.gameObject.scene;
                        simulationScope = RegisterSimulationScope(scene);
                    });

                using (subscription)
                {
                    var result = await networkRunner.StartGame(new StartGameArgs // this will connect and start loading the multiplayer scene of the host automatically
                    {
                        GameMode = GameMode.Client,
                        SessionName = args.sessionCode,
                        SceneManager = sceneManager
                    });

                    if (!result.Ok)
                    {
                        throw new Exception(
                            $"Game network component initialization failed. Shutdown reason: {result.ShutdownReason}. Custom error data: {result.ErrorMessage}. Stacktrace:\n{result.StackTrace}.");
                    }
                    
                    await UniTask.WaitUntil(() => SceneManager.GetSceneByBuildIndex(sceneRef.AsIndex).isLoaded); // networkRunner.StartGame returns before the scene is actually loaded
                    
                    await UniTask.WaitUntil(() => simulationScope != null); // wait for the simulation scope to be registered
                }
            }

            await UniTask.WaitWhile(() => networkRunner.IsStarting);
            
            var simulation = simulationScope.Container.Resolve<IGameSimulation>();
                
            return simulation;
            
            void RegisterInput(IContainerBuilder builder)
            {
                builder.RegisterInstance(args);
                builder.RegisterComponent(networkRunner);
            }

            GameSimulationScope RegisterSimulationScope(Scene scene)
            {
                var simulationScope = (GameSimulationScope)
                    LifetimeScope.Find<GameSimulationScope>(scene);

                using (LifetimeScope.EnqueueParent(_scope))
                using (LifetimeScope.Enqueue(RegisterInput))
                {
                    simulationScope.Build();
                }
                
                return simulationScope;
            }
        }
    }
}