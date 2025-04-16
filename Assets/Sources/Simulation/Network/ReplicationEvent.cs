using System;
using Fusion;
using KickinIt.Simulation.Game;
using KickinIt.Simulation.Player;
using R3;
using UnityEngine;

namespace KickinIt.Simulation.Network
{
    public class ReplicationEvent<TNetworkComponent> where TNetworkComponent : NetworkBehaviour
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