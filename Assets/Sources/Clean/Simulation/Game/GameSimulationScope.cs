using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Simulation
{
    public class GameSimulationScope : LifetimeScope
    {
        [SerializeField] private NetworkRunner networkRunner;
        [SerializeField] private GameNetwork network;
        // [SerializeField] private PlayerManager playerManagerPrefab;
        [SerializeField] private PlayerManager playerManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ISimulationConfig, HardcodeSimulationConfig>(Lifetime.Singleton);

            builder.Register<IGameSimulation, GameSimulation>(Lifetime.Singleton);
            
            builder.RegisterComponent(networkRunner);
            builder.RegisterComponent(network);
            // builder.Register<PlayerManagerFactory>(Lifetime.Singleton)
            //     .WithParameter(playerManagerPrefab);
            builder.RegisterComponent(playerManager);
            
            builder.Register<InputCollector>(Lifetime.Singleton);
        }
    }
}