namespace KickinIt.Simulation.Player
{
    internal struct PlayerHealthOverInfo
    {
        public HealthOverInfo OriginalInfo;
        public IPlayerSimulation Player;

        public PlayerHealthOverInfo(HealthOverInfo original, IPlayerSimulation player)
        {
            OriginalInfo = original;
            Player = player;
        }
    }
}