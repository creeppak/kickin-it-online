using Fusion;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Network
{
    public static class NetworkRunnerInstaller
    {
        public static IContainerBuilder RegisterNetworkRunnerComponents(this IContainerBuilder builder,
            NetworkRunner networkRunnerPrefab)
        {
            builder.Register<INetworkRunnerContainer, NetworkRunnerContainer>(Lifetime.Singleton);
            builder.RegisterFactory<NetworkRunner>(_ => () =>
            {
                var networkRunner = Object.Instantiate(networkRunnerPrefab);
                networkRunner.gameObject.name = "[M] NetworkRunner";
                return networkRunner;
            }, Lifetime.Singleton);
            return builder;
        }
    }
}