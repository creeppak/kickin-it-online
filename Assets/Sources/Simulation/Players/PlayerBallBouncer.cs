using System;
using System.Linq;
using Fusion;
using KickinIt.Simulation.Balls;
using KickinIt.Simulation.Input;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace KickinIt.Simulation.Players
{
    internal class PlayerBallBouncer : NetworkBehaviour
    {
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private LayerMask ballMask;
        [SerializeField] private float pushCooldown;
        [SerializeField] private float pushForceScale = 1.5f;
        [SerializeField] private UnityEvent pushedEvent;
        
        private readonly Collider[] _overlapBuffer = new Collider[8];
        private readonly Subject<int> _pushedSubject = new();
        private PhysicsScene _physicsScene;
        private Transform _colliderTransform;
        private int _localPushedTimes;

        [Networked] private float LastPushTime { get; set; }
        [Networked] private int PushedTimes { get; set; }
        
        public float PushCooldownNormalized => Mathf.Clamp01((Runner.SimulationTime - LastPushTime) / pushCooldown);
        public Observable<int> Pushed => _pushedSubject;

        [Inject]
        private void Construct(PhysicsScene physicsScene)
        {
            _physicsScene = physicsScene;
        }

        private void Awake()
        {
            _colliderTransform = capsuleCollider.transform;
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out KickingItNetworkInput inputData))
            {
                return;
            }
            
            if (!inputData.buttons.IsSet(KickingItButtons.Push)) // check button pressed
            {
                return;
            }

            if (Runner.SimulationTime - LastPushTime < pushCooldown) // check cooldown
            {
                return;
            }
            
            TriggerPush();
        }

        public override void Render()
        {
            if (_localPushedTimes < PushedTimes)
            {
                _localPushedTimes = PushedTimes;
                _pushedSubject.OnNext(_localPushedTimes);
                pushedEvent.Invoke();
            }
        }

        private void TriggerPush()
        {
            LastPushTime = Runner.SimulationTime;
            PushedTimes++;
            
            var capsuleDirection = DirectionToVector(capsuleCollider.direction);
            var worldCapsuleCenter = _colliderTransform.TransformPoint(capsuleCollider.center);
            var overlapCount = _physicsScene.OverlapCapsule(
                worldCapsuleCenter - capsuleDirection * capsuleCollider.height,
                worldCapsuleCenter + capsuleDirection * capsuleCollider.height,
                capsuleCollider.radius,
                _overlapBuffer,
                ballMask
            );

            if (overlapCount <= 0)
            {
                return;
            }
            
            var objectsToPush = Enumerable.Range(0, overlapCount).Select(i => _overlapBuffer[i]);

            foreach (var objectToPush in objectsToPush)
            {
                var ball = objectToPush.GetComponent<Ball>();
                
                if (!ball) continue;
                
                var pushDirection = (ball.transform.position - worldCapsuleCenter).normalized;
                var pushStrength = ball.CurrentMaxSpeed * pushForceScale;
                
                ball.Push(pushDirection * pushStrength);
                ball.IncrementMaxSpeedStep();
            }
        }

        private Vector3 DirectionToVector(int direction)
        {
            switch (direction)
            {
                case 0:
                    return Vector3.right;
                case 1:
                    return Vector3.up;
                case 2:
                    return Vector3.forward;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}