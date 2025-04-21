using Fusion;
using Sources.Common;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulation : IPlayerSimulation
    {
        private readonly PlayerScore _playerScore;
        private readonly PlayerReadinessSystem _playerReadinessSystem;
        private readonly FancyNameProvider _fancyNameProvider;
        private PlayerRef _playerRef;

        public NetworkObject NetworkObject { get; }

        public bool IsReady => _playerReadinessSystem.IsReady;

        public string PlayerName => _fancyNameProvider.GetName(_playerRef.AsIndex);

        public PlayerSimulation(PlayerScore playerScore, PlayerReadinessSystem playerReadinessSystem, NetworkObject networkObject, FancyNameProvider fancyNameProvider, PlayerRef playerRef)
        {
            _playerRef = playerRef;
            _fancyNameProvider = fancyNameProvider;
            NetworkObject = networkObject;
            _playerReadinessSystem = playerReadinessSystem;
            _playerScore = playerScore;
        }
        
        public void OnMatchStart()
        {
            _playerScore.ResetScore();
        }

        public void SetReady(bool isReady) => _playerReadinessSystem.SetReady(isReady);
    }
}