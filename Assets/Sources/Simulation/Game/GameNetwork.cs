using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using KickinIt.Simulation.Input;
using KickinIt.Simulation.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace KickinIt.Simulation.Game
{
    public class GameNetwork : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner _networkRunner;
        private InputCollector _inputCollector;
        private PlayerManager _playerManager;

        [Inject]
        private void Construct(NetworkRunner networkRunner, InputCollector inputCollector, PlayerManager playerManager)
        {
            _playerManager = playerManager;
            _networkRunner = networkRunner;
            _inputCollector = inputCollector;
        }
        
        internal async UniTask ShutdownSession()
        {
            // var sceneIndex = _networkRunner.SceneManager.GetSceneRef(gameObject).AsIndex;
            await _networkRunner.UnloadScene(GameSimulationConstants.SimulationSceneName);
            await _networkRunner.Shutdown();
            // await SceneManager.UnloadSceneAsync(sceneIndex);
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            var inputData = _inputCollector.CollectInput(runner);
            input.Set(inputData);
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            _playerManager.OnPlayerJoined(player);
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            _playerManager.OnPlayerLeft(player);
        }
        
        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}

        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}

        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner){}

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}

        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner){}

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner){}
    }
}