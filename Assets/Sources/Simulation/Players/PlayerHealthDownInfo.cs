namespace KickinIt.Simulation.Player
{
    internal struct PlayerHealthDownInfo
    {
        public HealthDownInfo OriginalInfo;
        public IPlayerSimulation Player;

        public PlayerHealthDownInfo(HealthDownInfo original, IPlayerSimulation player)
        {
            OriginalInfo = original;
            Player = player;
        }
    }
}