using Fusion;
using UnityEngine;
using VContainer;

namespace Sources.Clean.Simulation
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed = 1f;
        
        private Track _track;
        
        [Networked] public float X { get; private set; }

        [Inject]
        private void Construct(Track track)
        {
            _track = track;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out MyNetworkInput inputData))
            {
                var inputMovement = inputData.movement;

                // update 1D
                X += inputMovement * speed * Runner.DeltaTime;
            }

            X = _track.ClampPosition(X);
            
            // update 3D
            transform.position = _track.GetWorldPosition(X);
            transform.rotation = _track.GetRotation(X);
        }
    }
}