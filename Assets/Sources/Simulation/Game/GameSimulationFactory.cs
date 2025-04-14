using KickinIt.Simulation.Track;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Game
{
    // public class GameSimulationFactory
    // {
    //     private readonly LifetimeScope _scope;
    //     private readonly GameSimulationScope _prototype;
    //     private readonly TrackProvider _trackProvider;
    //
    //     public GameSimulationFactory(LifetimeScope scope, GameSimulationScope prototype, TrackProvider trackProvider)
    //     {
    //         _trackProvider = trackProvider;
    //         _prototype = prototype;
    //         _scope = scope;
    //     }
    //
    //     public IGameSimulation Create(SimulationArgs args)
    //     {
    //         GameSimulationScope gameSimulationScope;
    //         
    //         using (LifetimeScope.EnqueueParent(_scope))
    //         using (LifetimeScope.Enqueue(builder =>
    //                {
    //                    builder.RegisterInstance(args);
    //                    builder.RegisterComponent(_trackProvider);
    //                }))
    //         {
    //             gameSimulationScope = (GameSimulationScope) Object.Instantiate(_prototype, _scope.gameObject.scene);
    //             gameSimulationScope.Build();
    //         }
    //
    //         return gameSimulationScope.Container.Resolve<IGameSimulation>();
    //     }
    // }
}