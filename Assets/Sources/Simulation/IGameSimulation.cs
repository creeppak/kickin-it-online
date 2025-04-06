using Cysharp.Threading.Tasks;
using R3;

namespace Sources.Clean.Simulation
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