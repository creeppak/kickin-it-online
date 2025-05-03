using Fusion;
using KickinIt.Simulation.Synchronization;
using KickinIt.Simulation.Track;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulationScope : LifetimeScope
    {
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerHealth health;
        [SerializeField] private PlayerReadinessSystem readinessSystem;
        [SerializeField] private new PlayerCamera camera;

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
            builder.RegisterComponent(movement);
            builder.RegisterComponent(health)
                .As<IInitializable>()
                .AsSelf();
            builder.RegisterComponent(readinessSystem);
            builder.RegisterComponent(camera);
            
            builder.Register(ResolvePlayerTrack, Lifetime.Singleton);
            
            builder.UseEntryPoints(pointsBuilder =>
            {
                pointsBuilder.Add<PlayerSimulationBoot>();
                pointsBuilder.Add<ScopeInitializationManager>();
            });
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }

        private PlayerTrack ResolvePlayerTrack(IObjectResolver resolver)
        {
            var trackProvider = resolver.Resolve<TrackProvider>();
            var playerRef = resolver.Resolve<PlayerRef>();
            var track = trackProvider.GetTrack(playerRef.AsIndex - 1);
            resolver.InjectGameObject(track.gameObject);
            return track;
        }
    }
}