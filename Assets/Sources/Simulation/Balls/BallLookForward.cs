using System;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Balls
{
    public class BallLookForward : MonoBehaviour
    {
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
            _transform.forward = _ballMovement.Velocity.normalized;
        }
    }
}