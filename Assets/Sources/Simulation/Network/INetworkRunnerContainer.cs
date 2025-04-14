using Fusion;

namespace KickinIt.Simulation.Network
{
    internal interface INetworkRunnerContainer
    {
        NetworkRunner Current { get; }
        NetworkRunner InitializeNew();
        void Clear();
    }
}