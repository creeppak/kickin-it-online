using Fusion;

namespace KickinIt.Simulation.Input
{
    public interface IInputWriter
    {
        KickingItNetworkInput WriteInput(NetworkRunner networkRunner, KickingItNetworkInput inputData);
    }
}