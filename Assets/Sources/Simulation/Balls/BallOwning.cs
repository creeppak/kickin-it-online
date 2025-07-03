using Fusion;
using UnityEngine;

namespace KickinIt.Simulation.Balls
{
    public class BallOwning : NetworkBehaviour
    {
        [Networked] private PlayerRef OwnerNetworked { get; set; }
        
        public PlayerRef Owner => OwnerNetworked;

        private void OnCollisionEnter(Collision other)
        {
            if (!Object.HasStateAuthority) return;

            SetGameObjectAsOwner(other.gameObject);
        }

        public void TrySetOwner(GameObject pushInitiator)
        {
            if (pushInitiator == null) return;
            
            SetGameObjectAsOwner(pushInitiator);
        }

        private void SetGameObjectAsOwner(GameObject ownerGameObject)
        {
            var player = ownerGameObject.GetComponent<IPlayerSimulation>();
            
            if (player == null) return;

            OwnerNetworked = player.PlayerRef;
        }
    }
}