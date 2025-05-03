using System.Collections.Generic;
using VContainer.Unity;

namespace KickinIt.Simulation.Synchronization
{
    public class ScopeInitializationManager : IInitializable
    {
        private readonly IEnumerable<IInitializable> _components;

        public ScopeInitializationManager(IEnumerable<IInitializable> components) // todo: check if parent IInitializables are not getting injected here
        {
            _components = components;
        }

        public void Initialize()
        {
            foreach (var component in _components)
            {
                if (component == this) continue; // avoid infinite loop
                
                component.Initialize();
            }
        }
    }
}