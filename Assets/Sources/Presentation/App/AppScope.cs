using System;
using Fusion;
using KickinIt.Presentation.BackgroundWorker;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Simulation.Network;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Game
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private AppStateSceneCollection sceneGameStates;
        [SerializeField] private NetworkRunner networkRunnerPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SimpleAppStateFactory>(Lifetime.Transient);
            builder.Register<SceneAppStateFactory>(Lifetime.Transient)
                .AsSelf()
                .As<IHotLoadAppStateProvider>();
            builder.Register<NetworkedAppStateFactory>(Lifetime.Transient);
            builder.RegisterInstance<IAppStateSceneProvider>(sceneGameStates);
            builder.Register<IAppStateFactory, MasterAppStateFactory>(Lifetime.Transient);
            builder.Register<IAppStateManager, AppStateManager>(Lifetime.Singleton);
            builder.RegisterNetworkRunnerComponents(networkRunnerPrefab);
            
            // background worker
            {
                builder.RegisterInstance<IBackgroundWorkerConfig>(new BackgroundWorkerConfig
                    { BackgroundSceneName = "Background Scene" });
                
                builder.Register<BackgroundWorkerProxy>(Lifetime.Singleton)
                    .As<IBackgroundWorker>()
                    .AsSelf();

                builder.Register<BackgroundWorkerKeeper>(Lifetime.Singleton)
                    .AsSelf();
            }
            
            builder.UseEntryPoints(pointsBuilder =>
            {
                pointsBuilder.Add<BackgroundWorkerKeeper>();
                pointsBuilder.Add<AppBoot>();
            });
            
            builder.RegisterEntryPointExceptionHandler(OnGameBootException);
        }

        private void OnGameBootException(Exception obj)
        {
            Debug.LogException(obj);
        }
    }
}