using Fusion;
using R3;

namespace KickinIt.Simulation
{
    public interface IPlayerSimulation
    {
        public NetworkObject NetworkObject { get; }
        public void SetReady(bool isReady);
        public bool IsReady { get; }
    }
}