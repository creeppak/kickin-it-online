using Fusion;
using KickinIt.Simulation.Players;
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
        [SerializeField] private PlayerColor color;
        [SerializeField] private PlayerBallBouncer pushForce;
        [SerializeField] private new PlayerAnimation animation;
        [SerializeField] private PlayerDeathHandler deathHandler;

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
            builder.UseEntryPoints(pointsBuilder =>
            {
                pointsBuilder.Add<PlayerSimulationBoot>();
                pointsBuilder.Add<NetworkedInitializationManager<IPlayerInitializable>>();
            });
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
            
            builder.Register<IPlayerSimulation, PlayerSimulation>(Lifetime.Singleton); // facade
            builder.RegisterComponent(networkObject);
            builder.RegisterComponent(movement);
            builder.RegisterComponent(health).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponent(readinessSystem);
            builder.RegisterComponent(camera);
            builder.RegisterComponent(color);
            builder.RegisterComponent(pushForce);
            builder.RegisterComponent(animation);
            builder.RegisterComponent(deathHandler);
            builder.Register(ResolvePlayerTrack, Lifetime.Singleton);
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