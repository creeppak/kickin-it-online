using Fusion;

namespace KickinIt.Simulation.Player
{
    internal class PlayerScore : NetworkBehaviour
    {
        [Networked] public int HealthLeft { get; private set; }

        public void ResetScore()
        {
            HealthLeft = 5; // todo move to config
        }
    }
}