using Fusion;
using KickinIt.Simulation.Track;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulationScope : LifetimeScope
    {
        [FormerlySerializedAs("network")] [SerializeField] private PlayerReplication replication;
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerScore score;
        [SerializeField] private PlayerReadinessSystem readinessSystem;
        [SerializeField] private PlayerCamera camera;

        private void OnValidate()
        {
            if (autoRun)
            {
                autoRun = false;
                Debug.LogError($"Disabled auto-run option for {nameof(PlayerSimulationScope)} as it's required for correct network state synchronization.");
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IPlayerSimulation, PlayerSimulation>(Lifetime.Singleton); // facade
            
            builder.RegisterComponent(networkObject);
            builder.RegisterComponent(replication);
            builder.RegisterComponent(movement);
            builder.RegisterComponent(score);
            builder.RegisterComponent(readinessSystem);
            builder.RegisterComponent(camera);
            
            builder.Register(ResolvePlayerTrack, Lifetime.Singleton);
            
            // builder.Register<PlayerSimulationBoot>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<PlayerSimulationBoot>();
            
            // builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }

        private Track.PlayerTrack ResolvePlayerTrack(IObjectResolver resolver)
        {
            var trackProvider = resolver.Resolve<TrackProvider>();
            var playerRef = resolver.Resolve<PlayerRef>();
            return trackProvider.GetTrack(playerRef.AsIndex - 1);
        }
    }
}