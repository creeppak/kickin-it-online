using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;

namespace KickinIt.Simulation
{
    public interface IGameSimulation
    {
        UniTask StartSimulation();
        UniTask TerminateSimulation();
        Observable<SimulationPhase> Phase { get; }
        Observable<int> Countdown { get; }
        [CanBeNull] IPlayerSimulation GetPlayer(int index);
        string SessionCode { get; }
        UniTask EnsureLocalPlayerInitialized();
        Observable<Unit> PlayerJoined { get; }
        Observable<Unit> PlayerLeft { get; }
    }
}