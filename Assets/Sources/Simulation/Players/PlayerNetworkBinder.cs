namespace KickinIt.Simulation.Synchronization
{
    public class PlayerNetworkBinder : NetworkBinder
    {
        public override int DispatchOrder => 10;
    }
}