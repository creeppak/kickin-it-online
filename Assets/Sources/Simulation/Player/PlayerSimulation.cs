namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulation : IPlayerSimulation
    {
        private readonly PlayerScore _playerScore;

        public PlayerSimulation(PlayerScore playerScore)
        {
            _playerScore = playerScore;
        }
        
        public void OnMatchStart()
        {
            _playerScore.ResetScore();
        }
    }
}