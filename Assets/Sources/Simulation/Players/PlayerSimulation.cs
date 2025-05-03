using Fusion;
using KickinIt.Simulation.Track;
using R3;
using Sources.Common;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulation : IPlayerSimulation
    {
        private readonly PlayerHealth _playerHealth;
        private readonly PlayerReadinessSystem _playerReadinessSystem;
        private PlayerRef _playerRef;
        private FancyNameProvider _fancyNameProvider;

        public NetworkObject NetworkObject { get; }
        
        public PlayerRef PlayerRef => _playerRef;

        Observable<IPlayer> IPlayer.OnHealthUpdated => _playerHealth.OnHealthUpdated
            .Select(_ => this as IPlayer);

        Observable<IPlayer> IPlayer.OnHealthOver => _playerHealth.OnHealthOver
            .Select(_ => this as IPlayer);

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
            PlayerRef playerRef)
        {
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
    }
}