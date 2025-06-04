using Fusion;
using KickinIt.Simulation.Players;
using KickinIt.Simulation.Track;
using R3;
using Sources.Common;
using UnityEngine;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulation : IPlayerSimulation
    {
        private readonly PlayerHealth _playerHealth;
        private readonly PlayerReadinessSystem _playerReadinessSystem;
        private readonly FancyNameProvider _fancyNameProvider;
        private readonly PlayerColor _playerColor;
        private readonly PlayerBallBouncer _pushForce;
        
        private PlayerRef _playerRef;

        public NetworkObject NetworkObject { get; }
        
        public PlayerRef PlayerRef => _playerRef;

        Observable<IPlayer> IPlayer.OnHealthUpdated => _playerHealth.OnHealthUpdated
            .Select(_ => this as IPlayer);

        Observable<IPlayer> IPlayer.OnHealthOver => _playerHealth.OnHealthOver
            .Select(_ => this as IPlayer);

        public float PushCooldownNormalized => _pushForce.PushCooldownNormalized;

        public ReadOnlyReactiveProperty<Color> Color => _playerColor.MainColor;

        public Observable<IPlayerSimulation> OnHealthDown => _playerHealth.OnHealthDown
            .Select(_ => this as IPlayerSimulation);

        public Observable<IPlayerSimulation> OnHealthOver => _playerHealth.OnHealthOver
            .Select(_ => this as IPlayerSimulation);

        public bool IsReady => _playerReadinessSystem.IsReady;
        public string PlayerName => _fancyNameProvider.GetName(_playerRef.AsIndex - 1);
        public int PlayerIndex => _playerRef.AsIndex - 1;
        public int HealthPoints => _playerHealth.HealthPoints;

        public PlayerSimulation(
            PlayerHealth playerHealth,
            PlayerReadinessSystem playerReadinessSystem,
            NetworkObject networkObject, 
            FancyNameProvider fancyNameProvider,
            PlayerRef playerRef,
            PlayerColor playerColor,
            PlayerBallBouncer pushForce)
        {
            _pushForce = pushForce;
            _playerColor = playerColor;
            _playerRef = playerRef;
            _fancyNameProvider = fancyNameProvider;
            NetworkObject = networkObject;
            _playerReadinessSystem = playerReadinessSystem;
            _playerHealth = playerHealth;
        }

        public void SetReady(bool isReady) => _playerReadinessSystem.SetReady(isReady);

        public void ResetPlayer()
        {
            _playerHealth.ResetHealth();
            _playerHealth.SetImmortal(true); // all players are immortal at the start
        }

        public void SetImmortal(bool immortal) => _playerHealth.SetImmortal(immortal);
        
        public void InitializePlayer()
        {
            _playerColor.PickRandomColor();
        }
    }
}