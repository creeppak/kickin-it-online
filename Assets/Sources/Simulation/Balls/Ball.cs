using Fusion;
using UnityEngine;

namespace KickinIt.Simulation.Balls
{
    internal class Ball : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private float startMaxSpeed = 5f;
        [SerializeField] private float spawnSpeed = 1f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 0.5f;
        
        // [SerializeField] private float hitMaxSpeedBonusScale = 1.2f; todo: implement the mechanic
        
        [Networked] private float MaxSpeed { get; set; }

        public override void Spawned() // use INetworkInitializable if you need dependencies
        {
            Runner.SetIsSimulated(Object, true);
        }

        public void InitializeOnServer(Vector3 direction)
        {
            MaxSpeed = startMaxSpeed;
            rigidBody.velocity = direction * spawnSpeed;
        }

        public override void FixedUpdateNetwork()
        {
            var currentSpeed = rigidBody.velocity.magnitude;
            float newSpeed;

            if (currentSpeed >= MaxSpeed)
            {
                newSpeed = currentSpeed - deceleration * Runner.DeltaTime;
            }
            else // lower than max speed
            {
                newSpeed = currentSpeed + acceleration * Runner.DeltaTime;
            }
            
            rigidBody.velocity = rigidBody.velocity.normalized * newSpeed;
        }
    }
}