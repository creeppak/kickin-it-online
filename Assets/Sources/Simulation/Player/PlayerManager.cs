using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private readonly ReactiveProperty<bool> _allPlayersReady = new(false);
        
        private NetworkRunner _networkRunner;
        private LifetimeScope _lifetimeScope;
        private PlayerRegistry _playerRegistry;

        public int PlayerCount => _playerRegistry.PlayerCount;

        public bool AllPlayersReady => _playerRegistry.CollectAllPlayers().All(simulation => simulation.IsReady);

        [Inject]
        private void Construct(NetworkRunner networkRunner, PlayerRegistry playerRegistry, LifetimeScope lifetimeScope)
        {
            _playerRegistry = playerRegistry;
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
            
            _playerRegistry.RegisterPlayer(playerRef, playerObject);
            _allPlayersReady.Value = false;
        }

        public void TerminatePlayer(PlayerRef playerRef)
        {
            var playerObject = _playerRegistry.GetPlayer(playerRef).NetworkObject;
            
            _networkRunner.Despawn(playerObject); // this should trigger player scope disposal
            _playerRegistry.UnregisterPlayer(playerRef);
        }

        public bool HasPlayer(PlayerRef playerRef)
        {
            return _playerRegistry.HasPlayer(playerRef);
        }

        public IPlayerSimulation GetPlayer(PlayerRef playerRef)
        {
            return _playerRegistry.GetPlayer(playerRef);
        }

        public void UnreadyAll()
        {
            foreach (var player in _playerRegistry.CollectAllPlayers())
            {
                player.SetReady(false);
            }
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