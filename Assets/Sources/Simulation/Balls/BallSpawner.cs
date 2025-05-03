using System;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace KickinIt.Simulation.Balls
{
    internal class BallSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef ballPrefab;
        [SerializeField] private float spawnZoneRadius = 5f;
        
        private Transform _transform;

        [Networked] private Ball ActiveBall { get; set; }

        private void Awake()
        {
            _transform = transform;
        }

        private void OnDrawGizmos()
        {
            DrawGizmos(Color.grey);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(Color.green);
        }

        private void DrawGizmos(Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, spawnZoneRadius);
        }

        public void SpawnBall()
        {
            if (ActiveBall != null) throw new Exception("Only one ball can be active at a time.");
            
            var spawnAtRadius = Random.Range(0f, spawnZoneRadius);
            var spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            var spawnPosition = spawnRotation * new Vector3(spawnAtRadius, 0f, 0f);
            var worldSpawnPosition = _transform.transform.position + spawnPosition;
            var moveDirection = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Vector3.forward;

            var ballNetworkObject = Runner.Spawn(
                ballPrefab, 
                worldSpawnPosition, 
                Quaternion.identity, 
                onBeforeSpawned: (_, o) => o.GetComponent<Ball>().InitializeOnServer(moveDirection));
            ActiveBall = ballNetworkObject.GetComponent<Ball>();
        }

        private void DespawnBall()
        {
            if (ActiveBall == null) throw new Exception("No active ball to despawn.");
            
            Runner.Despawn(ActiveBall.Object);
            ActiveBall = null;
        }
        
        public void TryDespawnBall()
        {
            if (ActiveBall == null) return;

            DespawnBall();
        }

        [Button]
        private void RespawnBall()
        {
            if (!Object.HasStateAuthority)
            {
                Debug.Log("Only object with input authority can respawn the ball.");
                return;
            }
            
            TryDespawnBall();
            SpawnBall();
        }
    }
}