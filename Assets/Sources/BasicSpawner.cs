using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    
    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();
    private bool _firePressed;

    private void Update()
    {
        _firePressed = Input.GetMouseButton(0);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        var playerIndex = player.RawEncoded % runner.Config.Simulation.PlayerCount;
        var playerPosition = new Vector3(playerIndex * 3f, 0f, 0f);
        var playerObject = runner.Spawn(_playerPrefab, playerPosition, Quaternion.identity, player);
        
        _spawnedPlayers.Add(player, playerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!_spawnedPlayers.TryGetValue(player, out var playerObject)) return;
        
        runner.Despawn(playerObject);
        _spawnedPlayers.Remove(player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) // send input
    {
        var inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        var inputData = new OldNetworkInputData { Movement = inputMovement };
        inputData.Buttons.Set(OldNetworkInputData.FireButton, _firePressed);

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