using KickinIt.Simulation.Game;
using KickinIt.Simulation.Synchronization;
using VContainer;

namespace KickinIt.Simulation
{
    public static class SimulationFactoryInstaller
    {
        public static IContainerBuilder RegisterSimulationFactory(this IContainerBuilder builder)
        {
            builder.Register<IGameSimulationFactory, GameSimulationFactory>(Lifetime.Singleton);
            builder.Register<NetworkBindingService>(Lifetime.Singleton);
            return builder;
        }
    }
}