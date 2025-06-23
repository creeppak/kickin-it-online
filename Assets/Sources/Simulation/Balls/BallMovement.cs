using Fusion;
using R3;
using UnityEngine;

namespace KickinIt.Simulation.Balls
{
    internal class BallMovement : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private float startMaxSpeed = 5f;
        [SerializeField] private float spawnSpeed = 1f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 0.5f;
        [SerializeField] private float maxSpeedStepScale = 1f / 8f;

        [SerializeField, Sirenix.OdinInspector.ReadOnly]
        private float debugSpeed;
        
        private ReactiveProperty<int> _maxSpeedStepProperty = new(0);
        
        [Networked] private float MaxSpeedNetworked { get; set; }
        [Networked] private int MaxSpeedStepNetworked { get; set; }

        public int MaxSpeedStep
        {
            get => MaxSpeedStepNetworked;
            private set
            {
                MaxSpeedStepNetworked = value;
                _maxSpeedStepProperty.OnNext(value);
            }
        }
        
        public float CurrentMaxSpeed => MaxSpeedNetworked;
        public Vector3 Velocity => rigidBody.velocity;
        
        public Observable<int> MaxSpeedStepObservable => _maxSpeedStepProperty;

        public override void Spawned() // use INetworkInitializable if you need dependencies
        {
            Runner.SetIsSimulated(Object, true);
            MaxSpeedStep = 0;
        }

        public void InitializeOnServer(Vector3 direction)
        {
            MaxSpeedNetworked = startMaxSpeed;
            rigidBody.velocity = direction * spawnSpeed;
        }

        public override void FixedUpdateNetwork()
        {
            var currentSpeed = rigidBody.velocity.magnitude;
            float newSpeed;

            if (currentSpeed >= MaxSpeedNetworked)
            {
                newSpeed = currentSpeed - deceleration * Runner.DeltaTime;
            }
            else // lower than max speed
            {
                newSpeed = currentSpeed + acceleration * Runner.DeltaTime;
            }
            
            rigidBody.velocity = rigidBody.velocity.normalized * newSpeed;

#if UNITY_EDITOR
            debugSpeed = rigidBody.velocity.magnitude;      
#endif
        }

        public override void Render()
        {
            if (_maxSpeedStepProperty.CurrentValue != MaxSpeedStepNetworked) // update observables on clients also
            {
                _maxSpeedStepProperty.Value = MaxSpeedStepNetworked;
            }
        }

        public void Push(Vector3 velocity)
        {
            rigidBody.velocity = velocity;
        }

        public void IncrementMaxSpeedStep()
        {
            MaxSpeedStep++;
            MaxSpeedNetworked = startMaxSpeed * (1f + MaxSpeedStep * maxSpeedStepScale);
        }
    }
}