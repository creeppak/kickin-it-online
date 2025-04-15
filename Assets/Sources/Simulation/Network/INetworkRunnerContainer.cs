using Fusion;

namespace KickinIt.Simulation.Network
{
    public interface INetworkRunnerContainer
    {
        NetworkRunner Current { get; }
        NetworkRunner InitializeNew();
        void Clear();
    }
}