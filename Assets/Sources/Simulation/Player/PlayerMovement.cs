using Fusion;
using KickinIt.Simulation.Input;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed = 16f;
        
        private Track.Track _track;
        
        [Networked] private float X { get; set; }

        [Inject]
        private void Construct(Track.Track track)
        {
            _track = track;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out MyNetworkInput inputData))
            {
                var inputMovement = Mathf.Clamp(inputData.movement, -1f, 1f);

                // update 1D
                X += inputMovement * speed * Runner.DeltaTime;
            }

            X = _track.ClampPosition(X);
        }

        public override void Render()
        {
            // update 3D
            transform.position = _track.GetWorldPosition(X);
            transform.rotation = _track.GetRotation(X);
        }
    }
}