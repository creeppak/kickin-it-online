using Fusion;
using KickinIt.Simulation.Network;

namespace KickinIt.Simulation.Player
{
    internal class PlayerNetwork : NetworkBehaviour
    {
        public override void Spawned()
        {
            ReplicationEventBus<PlayerNetwork>.PushAwaken(this);
        }
    }
}