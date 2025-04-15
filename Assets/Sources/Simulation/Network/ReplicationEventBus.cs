using Fusion;
using R3;
using UnityEngine;

namespace KickinIt.Simulation.Network
{
    public static class ReplicationEventBus<TNetworkComponent> where TNetworkComponent : NetworkBehaviour
    {
        private static readonly Subject<TNetworkComponent> _replicatedOnClientSubject = new();
        
        public static Observable<TNetworkComponent> ReplicatedOnClient => _replicatedOnClientSubject;
        
        public static void PushSpawned(TNetworkComponent component)
        {
            if (component.Runner.IsServer) { return; }
            
            Debug.Log($"Pushing spawned event for {component.GetType().Name}. Frame: {Time.frameCount}");
            
            _replicatedOnClientSubject.OnNext(component);
        }
    }
}