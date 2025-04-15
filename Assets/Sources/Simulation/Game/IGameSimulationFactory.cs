using Cysharp.Threading.Tasks;

namespace KickinIt.Simulation.Game
{
    public interface IGameSimulationFactory
    {
        UniTask<IGameSimulation> Create(SimulationArgs args);
    }
}