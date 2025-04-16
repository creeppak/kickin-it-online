using Fusion;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulation : IPlayerSimulation
    {
        private readonly PlayerScore _playerScore;
        private readonly PlayerReadinessSystem _playerReadinessSystem;

        public NetworkObject NetworkObject { get; }

        public bool IsReady => _playerReadinessSystem.IsReady;

        public PlayerSimulation(PlayerScore playerScore, PlayerReadinessSystem playerReadinessSystem, NetworkObject networkObject)
        {
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