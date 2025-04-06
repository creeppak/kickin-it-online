using Fusion;
using KickinIt.Simulation.Track;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulationScope : LifetimeScope
    {
        [SerializeField] private PlayerNetwork network;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerScore score;

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
            builder.RegisterComponent(network);
            builder.RegisterComponent(movement);
            builder.RegisterComponent(score);
            
            builder.Register(ResolvePlayerTrack, Lifetime.Singleton);
            
            // builder.Register<PlayerSimulationBoot>(Lifetime.Singleton);
            
            // builder.RegisterEntryPoint<PlayerSimulationBoot>();
            
            // builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }

        private Track.Track ResolvePlayerTrack(IObjectResolver resolver)
        {
            var trackProvider = resolver.Resolve<TrackProvider>();
            var playerRef = resolver.Resolve<PlayerRef>();
            return trackProvider.GetTrack(playerRef.AsIndex - 1);
        }
    }
}