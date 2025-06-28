using Fusion;
using KickinIt.Simulation.Input;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private float baseSpeed = 16f;
        [SerializeField] private float inputThreshold = 0.1f;
        [SerializeField] private float attackTime = 1f / 6f;
        [SerializeField] private AnimationCurve attackCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float releaseTime = 1f / 12f;
        [SerializeField] private AnimationCurve releaseCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private AnimationCurve speedScaleByInput = AnimationCurve.Linear(0, 0, 1, 1); 
        [SerializeField] private float stopThreshold = 0.1f;

        private Track.PlayerTrack _track;
        private Transform _transform;
        
        [Networked] private float X { get; set; }
        [Networked] private float VelocityNetworked { get; set; }
        [Networked] private float InputPhaseStartTime { get; set; }
        [Networked] private bool WasAcceleratingLastTick { get; set; }
        [Networked] private bool InputEnabled { get; set; }
        
        private float InputPhaseTime => Runner.SimulationTime - InputPhaseStartTime;
        
        public float Velocity => VelocityNetworked;
        
        public float XNormalized => _track.GetNormalizedPosition(X);

        private void OnValidate()
        {
            if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        }

        [Inject]
        private void Construct(Track.PlayerTrack track)
        {
            _track = track;
            _transform = transform;
        }

        public override void Spawned()
        {
            Runner.SetIsSimulated(Object, true);
        }

        public override void FixedUpdateNetwork()
        {
            var input = 0f;
            
            if (InputEnabled && GetInput(out KickingItNetworkInput inputData))
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
                VelocityNetworked = inputDirection * speed;
            }
            else if (Mathf.Abs(VelocityNetworked) < stopThreshold)
            {
                VelocityNetworked = 0f;
            }
            else
            {
                if (WasAcceleratingLastTick)
                {
                    WasAcceleratingLastTick = false;
                    ResetInputPhaseTime();
                }
                
                VelocityNetworked *= releaseCurve.Evaluate(InputPhaseTime / releaseTime);
            }

            X += VelocityNetworked * Runner.DeltaTime;
            X = _track.ClampPosition(X);
            
            UpdatePosition3D();

            void ResetInputPhaseTime()
            {
                InputPhaseStartTime = Runner.SimulationTime;
            }
        }
        
        public void SetInputEnabled(bool inputEnabled)
        {
            InputEnabled = inputEnabled;
        }

        private void UpdatePosition3D()
        {
            if (Runner.IsServer)
            {
                rigidbody.position = _track.GetWorldPosition(X);
                rigidbody.rotation = _track.GetRotation(X);
            }
            else
            {
                _transform.position = _track.GetWorldPosition(X);
                _transform.rotation = _track.GetRotation(X);
            }
        }
    }
}