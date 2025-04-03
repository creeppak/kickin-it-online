using System;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeNetwork : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunner;

    private void OnValidate()
    {
        if (!_networkRunner) _networkRunner = GetComponent<NetworkRunner>();
    }

    public async void StartGameAsHost()
    {
        await StartGame(GameMode.Host);
    }

    public async void StartGameAsClient()
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
            SessionName = "CRP_TEST_SESSION",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    } 
}