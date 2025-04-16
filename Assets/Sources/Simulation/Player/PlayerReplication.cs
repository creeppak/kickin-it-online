using Fusion;
using KickinIt.Simulation.Game;
using KickinIt.Simulation.Network;

namespace KickinIt.Simulation.Player
{
    internal class PlayerReplication : NetworkBehaviour, IAfterSpawned
    {
        public void AfterSpawned() // using AfterSpawned as a dirty hack to execute this after GameReplication, todo: manage the execution order of such components
        {
            ReplicationEvent<PlayerReplication>.PushSpawned(this);
        }
    }
}