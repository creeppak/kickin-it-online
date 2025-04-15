using System;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using KickinIt.Simulation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Match
{
    public class GamePresentationScope : AppStateSceneScope
    { 
        [SerializeField] private ScreenId initialScreenId;
        [SerializeField] private ScreenPrototypeCollectionAsset screenPrototypes;
        [SerializeField] private ScreenNester screenNester;
        
        protected override void ConfigureGameStateScope(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBoot>()
                .WithParameter(initialScreenId)
                .As<IAsyncDisposable>();
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
            
            builder.Register<GamePresenter>(Lifetime.Singleton)
                .As<ISimulationProvider>()
                .AsSelf();
            
            builder.RegisterGameScreens(screenPrototypes, screenNester);
            builder.RegisterSimulationFactory();
        }
    }
}