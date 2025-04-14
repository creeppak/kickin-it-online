using Fusion;
using KickinIt.Simulation.Network;

namespace KickinIt.Simulation.Game
{
    public class GameLogicReplicationHandler : NetworkBehaviour
    {
        private void Awake()
        {
            ReplicationEventBus<GameLogicReplicationHandler>.PushAwaken(this);
        }
    }
}