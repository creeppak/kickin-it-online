using System;
using KickinIt.Presentation.BackgroundWorker;
using KickinIt.Presentation.Game.GameStates;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Game
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private GameStateSceneCollection sceneGameStates;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SimpleGameStateFactory>(Lifetime.Transient);
            builder.Register<SceneGameStateFactory>(Lifetime.Transient);
            builder.RegisterInstance<IGameStateSceneProvider>(sceneGameStates);
            builder.Register<IGameStateFactory, MasterGameStateFactory>(Lifetime.Transient);
            builder.Register<IGameStateManager, GameStateManager>(Lifetime.Singleton);
            
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
                pointsBuilder.Add<GameBoot>();
            });
            
            builder.RegisterEntryPointExceptionHandler(OnGameBootException);
        }

        private void OnGameBootException(Exception obj)
        {
            Debug.LogException(obj);
        }
    }
}