using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sources.KickinIt
{
    [RequireComponent(typeof(NetworkRunner))]
    public class KickinItNetwork : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner _networkRunner;
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private PlayerTrack _track;
        
        private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();
        
        private void OnValidate()
        {
            if (!_networkRunner) _networkRunner = GetComponent<NetworkRunner>();
        }
        
        public async void StartHost()
        {
            await StartGame(GameMode.Host);
        }

        public async void StartClient()
        {
            await StartGame(GameMode.Client);
        }

        private async Task StartGame(GameMode gameMode)
        {
            _networkRunner.ProvideInput = true;
            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
            {
                sceneInfo.AddSceneRef(scene);
            }

            await _networkRunner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = "KICKIN_IT_TEST_SESSION",
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            var playerObject = runner.Spawn(_playerPrefab, inputAuthority: player,
                onBeforeSpawned: (_, obj) => obj.GetComponent<Player>().Initialize(_track));
            _spawnedPlayers.Add(player, playerObject);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!_spawnedPlayers.TryGetValue(player, out var playerObject)) return;
        
            runner.Despawn(playerObject);
            _spawnedPlayers.Remove(player);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var inputMovement = Input.GetAxis("Horizontal");
            var inputData = new KickinItInputData { movement = inputMovement };

            input.Set(inputData);
        }
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}

        public void OnConnectedToServer(NetworkRunner runner){}

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}

        public void OnSceneLoadDone(NetworkRunner runner){}

        public void OnSceneLoadStart(NetworkRunner runner){}
    }
}