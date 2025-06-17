using System;
using UnityEngine;
using UnityEngine.Events;

namespace KickinIt.Simulation.Balls
{
    public class BallBody : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayerMask;
        [SerializeField] private UnityEvent onCollision;
        
        private void OnCollisionEnter(Collision other)
        {
            if ((collisionLayerMask.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }
            
            onCollision.Invoke();
        }

        public void PlayCollision()
        {
            onCollision.Invoke();
        }
    }
}