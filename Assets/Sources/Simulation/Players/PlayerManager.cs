using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using KickinIt.Simulation.Network;
using KickinIt.Simulation.Synchronization;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerManager : MonoBehaviour, IInitializable
    {
        [SerializeField] private NetworkPrefabRef playerPrefabRef;
        
        private readonly ReactiveProperty<bool> _allPlayersReady = new(false);
        private readonly Subject<Unit> _onPlayerJoined = new();
        private readonly Subject<Unit> _onPlayerLeft = new();
        
        private NetworkRunner _networkRunner;
        private LifetimeScope _lifetimeScope;
        private PlayerRegistry _playerRegistry;
        private NetworkBindingService _networkBindingService;

        public int PlayerCount => _playerRegistry.PlayerCount;

        public bool AllPlayersReady => _playerRegistry.CollectAllPlayers().All(simulation => simulation.IsReady);
        
        public Observable<Unit> PlayerJoined => _onPlayerJoined;
        public Observable<Unit> PlayerLeft => _onPlayerLeft;

        [Inject]
        private void Construct(NetworkRunner networkRunner, PlayerRegistry playerRegistry, LifetimeScope lifetimeScope, NetworkBindingService networkBindingService)
        {
            _networkBindingService = networkBindingService;
            _playerRegistry = playerRegistry;
            _lifetimeScope = lifetimeScope;
            _networkRunner = networkRunner;
        }

        public void Initialize()
        {
            _networkBindingService.Track<PlayerNetworkBinder>()
                .IgnoreOnErrorResume(Debug.LogException)
                .Subscribe(OnPlayerSpawnedOnClient)
                .AddTo(this);
        }

        private void OnPlayerSpawnedOnClient(PlayerNetworkBinder component)
        {
            if (_networkRunner.IsServer) return; // ignore for now
            
            BuildPlayerScope(component.Object, component.Object.InputAuthority);
        }

        public bool HasPlayer(PlayerRef playerRef)
            => _playerRegistry.HasPlayer(playerRef);

        public bool TryGetPlayer(PlayerRef playerRef, out IPlayerSimulation playerSimulation) 
            => _playerRegistry.TryGetPlayer(playerRef, out playerSimulation);

        public IPlayerSimulation GetPlayer(PlayerRef playerRef)
            => _playerRegistry.GetPlayer(playerRef);

        public List<IPlayerSimulation> CollectAllPlayers() 
            => _playerRegistry.CollectAllPlayers();

        public void UnreadyAll()
        {
            foreach (var player in _playerRegistry.CollectAllPlayers())
            {
                player.SetReady(false);
            }
        }

        public void OnPlayerJoined(PlayerRef player)
        {
            if (_networkRunner.IsServer)
            {
                InitializeNewPlayer(player);
            }
            
            _onPlayerJoined.OnNext(Unit.Default); // todo check if synced with clients
        }

        public void OnPlayerLeft(PlayerRef player)
        {
            if (_networkRunner.IsServer)
            {
                TerminatePlayer(player);
            }
            
            _onPlayerLeft.OnNext(Unit.Default);
        }

        private void InitializeNewPlayer(PlayerRef playerRef)
        {
            var playerObject = _networkRunner.Spawn(playerPrefabRef, inputAuthority: playerRef,
                onBeforeSpawned: (runner, o) => OnBeforeSpawnedOnServer(o, playerRef));
            
            _playerRegistry.RegisterPlayer(playerRef, playerObject);
            _allPlayersReady.Value = false;
        }

        private void TerminatePlayer(PlayerRef playerRef)
        {
            var playerObject = _playerRegistry.GetPlayer(playerRef).NetworkObject;
            
            _networkRunner.Despawn(playerObject); // this should trigger player scope disposal
            _playerRegistry.UnregisterPlayer(playerRef);
        }

        private PlayerSimulationScope BuildPlayerScope(NetworkObject obj, PlayerRef playerRef)
        {
            using (LifetimeScope.EnqueueParent(_lifetimeScope))
            using (LifetimeScope.Enqueue(builder => builder.RegisterInstance(playerRef)))
            {
                var scope = obj.GetComponent<PlayerSimulationScope>();
                scope.Build();
                return scope;
            }
        }

        private void OnBeforeSpawnedOnServer(NetworkObject obj, PlayerRef playerRef)
        {
            // Build Player Scope
            var scope = BuildPlayerScope(obj, playerRef);
            
            // Initialize Player Simulation
            var playerSimulation = scope.Container.Resolve<IPlayerSimulation>();
            playerSimulation.ResetPlayer();
        }
    }
}