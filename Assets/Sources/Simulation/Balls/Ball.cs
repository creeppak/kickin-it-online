using Fusion;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Balls
{
    internal class Ball : NetworkBehaviour
    {
        private BallMovement _ballMovement;
        private BallTrail _ballTrail;
        
        public float CurrentMaxSpeed => _ballMovement.CurrentMaxSpeed;

        [Inject]
        private void Construct(BallMovement ballMovement)
        {
            _ballMovement = ballMovement;
        }

        public void InitializeOnServer(Vector3 direction)
        {
            _ballMovement.InitializeOnServer(direction);
        }

        public void Push(Vector3 velocity) => _ballMovement.Push(velocity);

        public void IncrementMaxSpeedStep() => _ballMovement.IncrementMaxSpeedStep();
    }
}