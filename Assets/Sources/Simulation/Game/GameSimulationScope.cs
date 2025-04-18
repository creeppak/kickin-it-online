using Fusion;
using KickinIt.Simulation.Input;
using KickinIt.Simulation.Player;
using KickinIt.Simulation.Track;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Game
{
    public class GameSimulationScope : LifetimeScope
    {
        [SerializeField] private GameSimulation simulation;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerRegistry playerRegistry;
        [SerializeField] private TrackProvider trackProvider;

        private void OnValidate()
        {
            if (autoRun)
            {
                autoRun = false;
                Debug.LogError("Disabled autorun option. Build should be run manually.");
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(simulation)
                .As<IGameSimulation>();

            builder.Register(ResolveGameNetwork, Lifetime.Singleton);
            
            builder.RegisterComponent(playerManager);
            builder.RegisterComponent(playerRegistry);
            builder.RegisterComponent(trackProvider);
            
            builder.Register<InputCollector>(Lifetime.Singleton);

            builder.Register<IInputWriter, SimpleMoveInputWriter>(Lifetime.Singleton);

            GameNetwork ResolveGameNetwork(IObjectResolver resolver)
            {
                var runner = resolver.Resolve<NetworkRunner>();
                var gameNetwork = runner.gameObject.AddComponent<GameNetwork>();
                resolver.Inject(gameNetwork);
                return gameNetwork;
            }
        }
    }
}