using Fusion;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Balls
{
    internal class Ball : NetworkBehaviour
    {
        private BallMovement _ballMovement;
        private BallBody _ballBody;
        private BallOwning _ballOwning;

        public float CurrentMaxSpeed => _ballMovement.CurrentMaxSpeed;
        public PlayerRef CurrentOwner => _ballOwning.Owner;

        [Inject]
        private void Construct(BallMovement ballMovement, BallBody ballBody, BallOwning ballOwning)
        {
            _ballOwning = ballOwning;
            _ballBody = ballBody;
            _ballMovement = ballMovement;
        }

        public void InitializeOnServer(Vector3 direction)
        {
            _ballMovement.InitializeOnServer(direction);
        }

        public void Push(Vector3 velocity, GameObject pushInitiator)
        {
            _ballMovement.Push(velocity);
            _ballBody.PlayCollision(); // todo won't work for remote players 
            _ballOwning.TrySetOwner(pushInitiator);
        }

        public void IncrementMaxSpeedStep() => _ballMovement.IncrementMaxSpeedStep();
    }
}