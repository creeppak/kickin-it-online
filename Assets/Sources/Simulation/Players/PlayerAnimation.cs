using KickinIt.Simulation.Player;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Players
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IdleTime = Animator.StringToHash("IdleTime");
        
        [SerializeField] private Animator animator;
        
        private PlayerMovement _playerMovement;
        
        private float _idleTime = 0f;

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
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
    }
}