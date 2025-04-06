using Fusion;

namespace Sources.Clean.Simulation
{
    public interface IInputWriter
    {
        MyNetworkInput WriteInput(NetworkRunner networkRunner, MyNetworkInput inputData);
    }
}