using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Sources.Clean.Simulation
{
    public class GameNetwork : MonoBehaviour, INetworkRunnerCallbacks
    {
        private ISimulationConfig _simulationConfig;
        private NetworkRunner _networkRunner;
        private InputCollector _inputCollector;
        // private PlayerManagerFactory _playerManagerFactory;
        private PlayerManager _playerManager;

        [Inject]
        // private void Construct(ISimulationConfig simulationConfig, NetworkRunner networkRunner, InputCollector inputCollector, PlayerManagerFactory playerManagerFactory)
        private void Construct(ISimulationConfig simulationConfig, NetworkRunner networkRunner, InputCollector inputCollector, PlayerManager playerManager)
        {
            // _playerManagerFactory = playerManagerFactory;
            _playerManager = playerManager;
            _networkRunner = networkRunner;
            _simulationConfig = simulationConfig;
            _inputCollector = inputCollector;
        }
        
        internal async UniTask JoinSession(string sessionCode)
        {
            await StartGame(sessionCode, GameMode.Client);
        }

        internal async UniTask HostNewSession(string sessionCode)
        {
            await StartGame(sessionCode, GameMode.Host);
            
            // _playerManager = _playerManagerFactory.Create();
        }
        
        internal async UniTask ShutdownSession()
        {
            await _networkRunner.Shutdown();
        }

        private async UniTask StartGame(string sessionCode, GameMode gameMode)
        {
            _networkRunner.ProvideInput = true;
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

            if (!scene.IsValid)
            {
                throw new InvalidOperationException("Scene index is not valid.");
            }
            
            var sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);

            var result = await _networkRunner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = _simulationConfig.GameNetworkSessionPrefix + sessionCode,
                Scene = sceneInfo,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            if (!result.Ok)
            {
                throw new Exception(
                    $"Game network component initialization failed. Shutdown reason: {result.ShutdownReason}. Custom error data: {result.ErrorMessage}. Stacktrace:\n{result.StackTrace}.");
            }
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            var inputData = _inputCollector.CollectInput(runner);
            input.Set(inputData);
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!_networkRunner.IsServer) return;
            
            _playerManager.InitializeNewPlayer(player);
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!_networkRunner.IsServer) return;
            
            _playerManager.TerminatePlayer(player);
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