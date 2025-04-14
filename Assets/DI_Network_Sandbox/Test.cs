using System;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DI_Network_Sandbox
{
    public class Test : MonoBehaviour
    {
        public bool swapHostAndClient;
        public TestNetworkSceneManager sceneManager;
        
        public NetworkRunner networkRunnerPrefab;
        private NetworkRunner _networkRunner;

        private async void Awake()
        {
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            
            _networkRunner = Instantiate(networkRunnerPrefab);
            var useHost = Application.isEditor;
            if (swapHostAndClient) useHost = !useHost;
            var gameMode = useHost ? GameMode.Host : GameMode.Client;

            await _networkRunner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = "Test session",
                // Scene = sceneInfo,
                Scene = null, // don't load any scene
                SceneManager = sceneManager,
            });
            
            TestLog.Log("Game started");
            
            if (gameMode == GameMode.Host)
            {
                TestLog.Log("Starting scene load...");

                var loadSceneOperation = _networkRunner.LoadScene("TestScene_Network", LoadSceneMode.Additive);

                TestLog.Log("Loading scene...");

                await loadSceneOperation;

                TestLog.Log("Scene loaded");
            }
            else
            {
                TestLog.Log("Loading scene on client.");
                SceneManager.LoadScene("TestScene_Network", LoadSceneMode.Additive);
            }
        }

        private void OnDestroy()
        {
            if (!_networkRunner) return;
            if (!_networkRunner.IsRunning) return;
            
            _networkRunner.Shutdown();
        }
    }

    public static class TestLog
    {
        public static void Log(string message)
        {
            Debug.Log($"{Time.frameCount}: {message}");
        }
    }
}