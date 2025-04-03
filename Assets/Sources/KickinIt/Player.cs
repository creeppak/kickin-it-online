using Fusion;
using UnityEngine;

namespace Sources.KickinIt
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private float _speed = 5f;
        
        private PlayerTrack _track;
        private float _trackPosition;

        public void Initialize(PlayerTrack track)
        {
            _track = track;
        }
        
        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out KickinItInputData inputData)) return;
            
            // Movement
            var movement = inputData.movement;
            _trackPosition = Mathf.Clamp(
                _trackPosition + movement * _speed * Runner.DeltaTime,
                _track.MinPosition,
                _track.MaxPosition);
            transform.position = _track.GetPosition(_trackPosition);
        }
    }
}