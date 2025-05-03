using System.Collections.Generic;
using VContainer.Unity;

namespace KickinIt.Simulation.Synchronization
{
    public class NetworkedInitializationManager<TInitializable> : IInitializable
        where TInitializable : INetworkedInitializable
    {
        private readonly IEnumerable<TInitializable> _components;

        public NetworkedInitializationManager(IEnumerable<TInitializable> components) // todo: check if parent IInitializables are not getting injected here
        {
            _components = components;
        }

        public void Initialize()
        {
            foreach (var component in _components)
            {
                component.Initialize();
            }
        }
    }
}