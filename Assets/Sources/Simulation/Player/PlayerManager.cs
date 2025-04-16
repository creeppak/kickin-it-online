using System.Collections.Generic;
using Fusion;
using KickinIt.Simulation.Network;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerManager : MonoBehaviour
    {
        [SerializeField] private NetworkPrefabRef playerPrefabRef;

        private readonly Dictionary<PlayerRef, NetworkObject> _playerObjects = new();
        
        private NetworkRunner _networkRunner;
        private LifetimeScope _lifetimeScope;
        
        private ReactiveProperty<int> _playerCount { get; } = new(0);
        
        public Observable<int> PlayerCount => _playerCount; 

        [Inject]
        private void Construct(NetworkRunner networkRunner, LifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            _networkRunner = networkRunner;
        }

        private void Awake()
        {
            ReplicationEvent<PlayerReplication>.ReplicatedOnClient
                .IgnoreOnErrorResume(Debug.LogException)
                .Subscribe(OnPlayerSpawnedOnClient)
                .AddTo(this);
        }

        private void OnPlayerSpawnedOnClient(PlayerReplication replicationComponent)
        {
            if (replicationComponent.Runner != _networkRunner) return;
            
            BuildPlayerScope(replicationComponent.Object, replicationComponent.Object.InputAuthority);
        }

        public void InitializeNewPlayer(PlayerRef playerRef)
        {
            var playerObject = _networkRunner.Spawn(playerPrefabRef, inputAuthority: playerRef,
                onBeforeSpawned: (runner, o) => OnBeforeSpawnedOnServer(runner, o, playerRef));
            _playerObjects.Add(playerRef, playerObject);
            
            _playerCount.Value = _playerObjects.Count;
        }

        public void TerminatePlayer(PlayerRef playerRef)
        {
            if (!_playerObjects.TryGetValue(playerRef, out var playerObject))
            {
                Debug.LogError("Tried despawning player object as part of termination procedure, but object was not found.");
                return;
            }
            
            _networkRunner.Despawn(playerObject); // this should trigger player scope disposal
            _playerObjects.Remove(playerRef);
            
            _playerCount.Value = _playerObjects.Count;
        }

        private void OnBeforeSpawnedOnServer(NetworkRunner runner, NetworkObject obj, PlayerRef playerRef)
        {
            BuildPlayerScope(obj, playerRef);
        }

        private void BuildPlayerScope(NetworkObject obj, PlayerRef playerRef)
        {
            using (LifetimeScope.EnqueueParent(_lifetimeScope))
            using (LifetimeScope.Enqueue(builder => builder.RegisterInstance(playerRef)))
            {
                var scope = obj.GetComponent<PlayerSimulationScope>();
                scope.Build();
            }
        }
    }
}