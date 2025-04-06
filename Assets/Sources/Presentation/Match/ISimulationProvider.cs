using KickinIt.Simulation;
using R3;

namespace KickinIt.Presentation.Match
{
    public interface ISimulationProvider
    {
        Observable<IGameSimulation> SimulationReady { get; }
        IGameSimulation Simulation { get; }
    }
}