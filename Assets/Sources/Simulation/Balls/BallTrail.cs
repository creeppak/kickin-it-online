using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Balls
{
    internal class BallTrail : MonoBehaviour
    {
        [SerializeField] private float activateFxAtSpeed = 10f;
        [SerializeField] private ParticleSystem fx;
        
        private BallMovement _ballMovement;
        private Transform _transform;

        [Inject]
        private void Construct(BallMovement ballMovement)
        {
            _ballMovement = ballMovement;
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.forward = -_ballMovement.Velocity.normalized;
            
            var shouldEmit = _ballMovement.Velocity.magnitude > activateFxAtSpeed;
            
            if (shouldEmit && !fx.isPlaying)
            {
                fx.Play();
            }
            else if (!shouldEmit && fx.isPlaying)
            {
                fx.Stop();
            }
        }
    }
}