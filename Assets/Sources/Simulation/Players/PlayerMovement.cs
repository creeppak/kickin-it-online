using Fusion;
using KickinIt.Simulation.Input;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float baseSpeed = 16f;
        [SerializeField] private float inputThreshold = 0.1f;
        [SerializeField] private float attackTime = 1f / 6f;
        [SerializeField] private AnimationCurve attackCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float releaseTime = 1f / 12f;
        [SerializeField] private AnimationCurve releaseCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private AnimationCurve speedScaleByInput = AnimationCurve.Linear(0, 0, 1, 1); 
        [SerializeField] private float stopThreshold = 0.1f;

        private Track.PlayerTrack _track;
        
        [Networked] private float X { get; set; }
        [Networked] private float Velocity { get; set; }
        [Networked] private float InputPhaseStartTime { get; set; }
        [Networked] private bool WasAcceleratingLastTick { get; set; }
        
        private float InputPhaseTime => Runner.SimulationTime - InputPhaseStartTime;

        [Inject]
        private void Construct(Track.PlayerTrack track)
        {
            _track = track;
        }

        public override void FixedUpdateNetwork()
        {
            var input = 0f;
            
            if (GetInput(out MyNetworkInput inputData))
            {
                input = Mathf.Clamp(inputData.movement, -1f, 1f);
            }
            
            var accelerate = Mathf.Abs(input) > inputThreshold;

            if (accelerate)
            {
                if (!WasAcceleratingLastTick)
                {
                    WasAcceleratingLastTick = true;
                    ResetInputPhaseTime();
                }
                
                var inputDirection = Mathf.Sign(input);
                var inputSpeed = baseSpeed * speedScaleByInput.Evaluate(
                    Mathf.InverseLerp(inputThreshold, 1f, Mathf.Abs(input)) // remap to (inputThreshold, 1.0)
                );
                var speed = InputPhaseTime < attackTime
                    ? attackCurve.Evaluate(InputPhaseTime / attackTime) * inputSpeed // attack curve
                    : inputSpeed; // hold curve
                Velocity = inputDirection * speed;
            }
            else if (Mathf.Abs(Velocity) < stopThreshold)
            {
                Velocity = 0f;
            }
            else
            {
                if (WasAcceleratingLastTick)
                {
                    WasAcceleratingLastTick = false;
                    ResetInputPhaseTime();
                }
                
                Velocity *= releaseCurve.Evaluate(InputPhaseTime / releaseTime);
            }

            X += Velocity * Runner.DeltaTime;
            X = _track.ClampPosition(X);

            void ResetInputPhaseTime()
            {
                InputPhaseStartTime = Runner.SimulationTime;
            }
        }

        public override void Render()
        {
            // update 3D
            transform.position = _track.GetWorldPosition(X);
            transform.rotation = _track.GetRotation(X);
        }
    }
}