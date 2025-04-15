using Fusion;
using KickinIt.Simulation.Network;

namespace KickinIt.Simulation.Game
{
    public class GameReplication : NetworkBehaviour
    {
        public override void Spawned()
        {
            ReplicationEventBus<GameReplication>.PushSpawned(this);
        }
    }
}