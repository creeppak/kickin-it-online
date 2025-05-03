using Fusion;

namespace KickinIt.Simulation.Synchronization
{
    public abstract class NetworkBinder : NetworkBehaviour
    {
        public abstract int DispatchOrder { get; }

        public sealed override void Spawned()
        {
            NetworkBindingService.PushSpawned(GetType(), this);
        }
    }
}