using Fusion;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using KickinIt.Simulation.Game;
using KickinIt.Simulation.Track;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Match
{
    public class GameScope : AppStateSceneScope
    { 
        [SerializeField] private ScreenId initialScreenId;
        [SerializeField] private ScreenPrototypeCollectionAsset screenPrototypes;
        [SerializeField] private ScreenNester screenNester;
        [FormerlySerializedAs("simulationScopePrototype")] [SerializeField] private GameSimulationScope gameSimulationScopePrototype;
        [SerializeField] private TrackProvider trackProvider;
		[SerializeField] private GameSimulationInstaller gameSimulationInstaller;
        
        protected override void ConfigureGameStateScope(IContainerBuilder builder)
        {
            builder.Register<GamePresenter>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.RegisterGameScreens(screenPrototypes, screenNester);
			// gameSimulationInstaller.RegisterGameS

            // builder.Register<GameSimulationFactory>(Lifetime.Singleton)
            //     .WithParameter(gameSimulationScopePrototype)
            //     .WithParameter(trackProvider);

            // register network runner
            builder.Register(resolver => resolver.Resolve<NetworkedAppStateArgs>().networkRunner, Lifetime.Singleton);
            
            builder.Register<GameBoot>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<GameBoot>()
                .WithParameter(initialScreenId);
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }
    }
}