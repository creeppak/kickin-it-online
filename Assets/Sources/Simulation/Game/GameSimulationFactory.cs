using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Simulation
{
    public class GameSimulationFactory
    {
        private readonly LifetimeScope _scope;
        private readonly GameSimulationScope _prototype;
        private readonly TrackProvider _trackProvider;

        public GameSimulationFactory(LifetimeScope scope, GameSimulationScope prototype, TrackProvider trackProvider)
        {
            _trackProvider = trackProvider;
            _prototype = prototype;
            _scope = scope;
        }

        public IGameSimulation Create(SimulationArgs args)
        {
            GameSimulationScope simulationScope;
            
            using (LifetimeScope.EnqueueParent(_scope))
            using (LifetimeScope.Enqueue(builder =>
                   {
                       builder.RegisterInstance(args);
                       builder.RegisterComponent(_trackProvider);
                   }))
            {
                simulationScope = (GameSimulationScope) Object.Instantiate(_prototype, _scope.gameObject.scene);
            }

            return simulationScope.Container.Resolve<IGameSimulation>();
        }
    }
}