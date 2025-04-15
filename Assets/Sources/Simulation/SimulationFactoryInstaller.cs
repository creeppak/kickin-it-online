using KickinIt.Simulation.Game;
using KickinIt.Simulation.Track;
using VContainer;

namespace KickinIt.Simulation
{
    public static class SimulationFactoryInstaller
    {
        public static IContainerBuilder RegisterSimulationFactory(this IContainerBuilder builder)
        {
            builder.Register<IGameSimulationFactory, GameSimulationFactory>(Lifetime.Singleton);
            return builder;
        }
    }
}