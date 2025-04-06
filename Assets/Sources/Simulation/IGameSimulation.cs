using Cysharp.Threading.Tasks;
using R3;

namespace KickinIt.Simulation
{
    public interface IGameSimulation
    {
        UniTask StartSimulation();
        UniTask TerminateSimulation();
        Observable<SimulationPhase> Phase { get; }
        Observable<int> Countdown { get; }
        IPlayerSimulation GetPlayer(int index);
        int PlayerCount { get; }
        string SessionCode { get; }
    }
}