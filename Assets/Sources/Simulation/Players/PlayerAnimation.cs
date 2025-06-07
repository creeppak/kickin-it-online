using System;
using KickinIt.Simulation.Player;
using R3;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Players
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IdleTime = Animator.StringToHash("IdleTime");
        private static readonly int Push = Animator.StringToHash("Push");
        
        [SerializeField] private Animator animator;
        
        private PlayerMovement _playerMovement;
        private PlayerBallBouncer _ballBouncer;
        
        private float _idleTime = 0f;

        [Inject]
        private void Construct(PlayerMovement playerMovement, PlayerBallBouncer ballBouncer)
        {
            _ballBouncer = ballBouncer;
            _playerMovement = playerMovement;
        }

        private void Start()
        {
            _ballBouncer.Pushed
                .Subscribe(_ => OnPush())
                .AddTo(this);
        }

        private void Update()
        {
            var velocity = _playerMovement.Velocity;
            animator.SetFloat(Speed, velocity);
            
            if (!Mathf.Approximately(Mathf.Abs(velocity), 0f))
            {
                _idleTime = 0f;
            }
            else
            {
                _idleTime += Time.deltaTime;
            }
            
            animator.SetFloat(IdleTime, _idleTime);
        }

        private void OnPush()
        {
            animator.SetTrigger(Push);
        }
    }
}