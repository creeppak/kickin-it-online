using System;
using Cysharp.Threading.Tasks;
using Fusion;

namespace KickinIt.Simulation.Network
{
    public struct ConnectionArgs
    {
        public bool host;
        public string sessionCode;
    }
    
    public class ConnectionSystem
    {
        private readonly INetworkRunnerContainer _networkRunnerContainer;

        internal ConnectionSystem(INetworkRunnerContainer networkRunnerContainer)
        {
            _networkRunnerContainer = networkRunnerContainer;
        }

        public async UniTask<NetworkRunner> InitiateNetworkConnection(ConnectionArgs args)
        {
            var networkRunner = _networkRunnerContainer.InitializeNew();

            try
            {
                networkRunner.ProvideInput = true;
                // var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

                // if (!scene.IsValid)
                // {
                //     throw new InvalidOperationException("Scene index is not valid.");
                // }
                //
                // var sceneInfo = new NetworkSceneInfo();
                // sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
                
                // remove built-in automatic scene loading
                // load app state, wrap loading
                // listen for Replication event, build simulation scope

                var result = await networkRunner.StartGame(new StartGameArgs
                {
                    GameMode = args.host ? GameMode.Host : GameMode.Client,
                    SessionName = SimulationConstants.PhotonSessionPrefix + args.sessionCode,
                    // Scene = sceneInfo,
                    SceneManager = args.host ? networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>() : null
                });

                if (!result.Ok)
                {
                    throw new Exception(
                        $"Game network component initialization failed. Shutdown reason: {result.ShutdownReason}. Custom error data: {result.ErrorMessage}. Stacktrace:\n{result.StackTrace}.");
                }

                return networkRunner;
            }
            catch
            {
                _networkRunnerContainer.Clear();
                throw;
            }
        }
    }
}