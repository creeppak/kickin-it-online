using Fusion;
using R3;
using UnityEngine;

namespace Sources.Clean.Simulation
{
    internal class PlayerNetwork : NetworkBehaviour
    {
        private static readonly Subject<(NetworkRunner runner, NetworkObject obj)> _playerSpawnedOnClient = new();

        public static Observable<(NetworkRunner runner, NetworkObject obj)> PlayerSpawnedOnClient => _playerSpawnedOnClient;

        public override void Spawned()
        {
            Debug.Log("I'm called");
            
            if (!Runner.IsServer)
            {
                _playerSpawnedOnClient.OnNext((Runner, Object));
            }
        }
    }
}