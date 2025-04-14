using Fusion;
using KickinIt.Simulation.Input;
using KickinIt.Simulation.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Simulation.Game
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
            
            // builder.RegisterComponentInHierarchy<>()
            
            builder.RegisterComponent(networkRunner);
            builder.RegisterComponent(network);
            // builder.Register<PlayerManagerFactory>(Lifetime.Singleton)
            //     .WithParameter(playerManagerPrefab);
            builder.RegisterComponent(playerManager);
            
            builder.Register<InputCollector>(Lifetime.Singleton);

            builder.Register<IInputWriter, SimpleMoveInputWriter>(Lifetime.Singleton);
        }
    }
}