using Sources.Clean.Simulation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Presentation
{
    public class MatchScope : GameStateSceneScope
    {
        [SerializeField] private ScreenId initialScreenId;
        [SerializeField] private ScreenPrototypeCollectionAsset screenPrototypes;
        [SerializeField] private ScreenNester screenNester;
        [SerializeField] private GameSimulationScope simulationScopePrototype;
        [SerializeField] private TrackProvider trackProvider;
        
        protected override void ConfigureGameStateScope(IContainerBuilder builder)
        {
            builder.Register<MatchPresentation>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.RegisterGameScreens(screenPrototypes, screenNester);
            builder.Register<GameSimulationFactory>(Lifetime.Singleton)
                .WithParameter(simulationScopePrototype)
                .WithParameter(trackProvider);
            
            builder.Register<MatchBoot>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<MatchBoot>()
                .WithParameter(initialScreenId);
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }
    }
}