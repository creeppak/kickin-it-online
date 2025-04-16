using Fusion;

namespace KickinIt.Simulation.Player
{
    internal class PlayerReadinessSystem : NetworkBehaviour
    {
        public bool IsReady => IsReadyNetworked;
        
        [Networked] private bool IsReadyNetworked { get; set; }

        public void SetReady(bool ready)
        {
            RPC_SetReady(ready);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetReady(bool ready)
        {
            IsReadyNetworked = ready;
        }
    }
}