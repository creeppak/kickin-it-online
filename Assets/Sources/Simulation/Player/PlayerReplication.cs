using Fusion;
using KickinIt.Simulation.Network;

namespace KickinIt.Simulation.Player
{
    internal class PlayerReplication : NetworkBehaviour
    {
        public override void Spawned()
        {
            ReplicationEventBus<PlayerReplication>.PushSpawned(this);
        }
    }
}