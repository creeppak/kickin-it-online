using Fusion;

namespace KickinIt.Simulation.Input
{
    public enum KickingItButtons
    {
        Push = 0,
    }
    
    public struct KickingItNetworkInput : INetworkInput
    {
        public float movement;
        public NetworkButtons buttons;
    }
}