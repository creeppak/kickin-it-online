using Fusion;
using R3;

namespace KickinIt.Simulation.Player
{
    internal class PlayerReadinessSystem : NetworkBehaviour
    {
        public ReadOnlyReactiveProperty<bool> IsReady => _isReady;
        
        [Networked] private bool IsReadyNetworked { get; set; }

        private readonly ReactiveProperty<bool> _isReady = new();

        public override void Render()
        {
            _isReady.Value = IsReadyNetworked;
        }

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