using System;
using Fusion;
using Object = UnityEngine.Object;

namespace KickinIt.Simulation.Network
{
    internal class NetworkRunnerContainer : INetworkRunnerContainer
    {
        private readonly Func<NetworkRunner> _networkRunnerFactory;
        
        private NetworkRunner _current;
        
        public NetworkRunner Current => _current;

        public NetworkRunnerContainer(Func<NetworkRunner> networkRunnerFactory)
        {
            _networkRunnerFactory = networkRunnerFactory;
        }

        public NetworkRunner InitializeNew()
        {
            if (_current != null)
            {
                throw new InvalidOperationException("NetworkRunner is already initialized.");
            }

            _current = _networkRunnerFactory.Invoke();
            Object.DontDestroyOnLoad(_current);
            return _current;
        }

        public void Clear()
        {
            Object.Destroy(_current.gameObject);
            _current = null;
        }
    }
}