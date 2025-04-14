using Fusion;
using R3;

namespace KickinIt.Simulation.Network
{
    public static class ReplicationEventBus<TNetworkComponent> where TNetworkComponent : NetworkBehaviour
    {
        private static readonly Subject<TNetworkComponent> _replicatedOnClientSubject = new();
        
        public static Observable<TNetworkComponent> ReplicatedOnClient => _replicatedOnClientSubject;
        
        public static void PushAwaken(TNetworkComponent component)
        {
            if (component.Runner.IsServer) { return; }
            
            _replicatedOnClientSubject.OnNext(component);
        }
    }
}