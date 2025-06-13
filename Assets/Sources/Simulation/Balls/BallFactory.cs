using Fusion;
using UnityEngine;
using VContainer.Unity;

namespace KickinIt.Simulation.Balls
{
    internal class BallFactory
    {
        private readonly LifetimeScope _scope;
        private readonly NetworkRunner _runner;

        public BallFactory(LifetimeScope scope, NetworkRunner runner)
        {
            _runner = runner;
            _scope = scope;
        }

        public Ball Create(NetworkPrefabRef ballPrefab, Vector3 spawnPosition, Vector3 moveDirection)
        {
            using (LifetimeScope.EnqueueParent(_scope))
            {
                var newBall = _runner.Spawn(
                    ballPrefab, 
                    spawnPosition, 
                    Quaternion.identity, 
                    onBeforeSpawned: (_, o) => o.GetComponent<Ball>().InitializeOnServer(moveDirection));
                
                return newBall.GetComponent<Ball>();
            }
        }
    }
}