using Fusion;

namespace KickinIt.Simulation.Input
{
    public interface IInputWriter
    {
        MyNetworkInput WriteInput(NetworkRunner networkRunner, MyNetworkInput inputData);
    }
}